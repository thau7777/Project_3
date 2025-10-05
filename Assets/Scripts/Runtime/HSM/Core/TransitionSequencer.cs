using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace HSM {
    public class TransitionSequencer {
        public readonly StateMachine Machine;
        
        ISequence sequencer;                 // current phase (deactivate or activate)
        Action nextPhase;                    // switch structure between phases
        (State from, State to)? pending;     // coalesce a single pending request
        State lastFrom, lastTo;
        
        CancellationTokenSource cts;
        bool UseSequential = false;          // set false to use parallel

        public TransitionSequencer(StateMachine machine) {
            Machine = machine;
        }

        // Request a transition from one state to another
        public void RequestTransition(State from, State to) {
            if (to == null || from == to) return;
            if (sequencer != null){ pending = (from, to); return; }
            BeginTransition(from, to);
        }

        static List<PhaseStep> GatherPhaseSteps(List<State> chain, bool deactivate) {
            var steps = new List<PhaseStep>();

            for (int i = 0; i < chain.Count; i++) {
                var st = chain[i];
                var acts = chain[i].Activities;
                for (int j = 0; j < acts.Count; j++){
                    var a = acts[j];
                    bool include = deactivate ? (a.Mode == ActivityMode.Active)
                        : (a.Mode == ActivityMode.Inactive);
                    if (!include) continue;

                    

                    steps.Add(ct => deactivate ? a.DeactivateAsync(ct) : a.ActivateAsync(ct));
                }
            }
            return steps;
        }
        
        // States to exit: from → ... up to (but excluding) lca; bottom→up order.
        static List<State> StatesToExit(State from, State lca) {
            var list = new List<State>();
            for (var s = from; s != null && s != lca; s = s.Parent) list.Add(s);
            return list;
        }
        
        // States to enter: path from 'to' up to (but excluding) lca; returned in enter order (top→down).
        static List<State> StatesToEnter(State to, State lca) {
            var stack = new Stack<State>();
            for (var s = to; s != lca; s = s.Parent) stack.Push(s);
            return new List<State>(stack);
        }

        void BeginTransition(State from, State to) {
            cts?.Cancel();
            cts = new CancellationTokenSource();
            
            var lca        = Lca(from, to);
            var exitChain  = StatesToExit(from, lca);
            var enterChain = StatesToEnter(to,  lca);
            
            // 1. Deactivate the “old branch”
            var exitSteps  = GatherPhaseSteps(exitChain, deactivate: true);
            // sequencer = new NoopPhase();
            sequencer = UseSequential
                ? new SequentialPhase(exitSteps, cts.Token)
                : new ParallelPhase(exitSteps, cts.Token);
            sequencer.Start();
            
            nextPhase = () => {
                // 2. ChangeState
                Machine.ChangeState(from, to);
                // 3. Activate the “new branch”
                var enterSteps = GatherPhaseSteps(enterChain, deactivate: false);
                // sequencer = new NoopPhase();
                sequencer = UseSequential
                    ? new SequentialPhase(enterSteps, cts.Token)
                    : new ParallelPhase(enterSteps, cts.Token);
                sequencer.Start();
            };
        }

        void EndTransition() {
            sequencer = null;

            if (pending.HasValue) {
                (State from, State to) p = pending.Value;
                pending = null;
                BeginTransition(p.from, p.to);
            }
        }

        public void Tick(float deltaTime) {
            if (sequencer != null) {
                if (sequencer.Update()) {
                    if (nextPhase != null) {
                        var n = nextPhase;
                        nextPhase = null;
                        n();
                    } else {
                        EndTransition();
                    }
                }
                return; // while transitioning, we don't run normal updates
            }
            Machine.InternalTick(deltaTime);
        }

        // Compute the Lowest Common Ancestor of two states.
        public static State Lca(State a, State b) {
            // Create a set of all parents of 'a'
            var ap = new HashSet<State>();
            for (var s = a; s != null; s = s.Parent) ap.Add(s);

            // Find the first parent of 'b' that is also a parent of 'a'
            for (var s = b; s != null; s = s.Parent)
                if (ap.Contains(s))
                    return s;

            // If no common ancestor found, return null
            return null;
        }
    }
}