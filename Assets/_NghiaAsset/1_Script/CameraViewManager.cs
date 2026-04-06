using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;


namespace Turnbase
{
    public class CameraViewManager : MonoBehaviour
    {
        public RawImage CameraView;
        private Character _lastActiveCharacter;

        public void SetCameraView(Character character)
        {
            // Disable the selfie camera of the previous character
            if (_lastActiveCharacter != null)
            {
                _lastActiveCharacter.SetSelfieCameraActive(false);
            }

            if (character == null || character.RenderTexture == null)
            {
                CameraView.texture = null;
                _lastActiveCharacter = null;
                return;
            }

            // Enable the selfie camera of the target character
            character.SetSelfieCameraActive(true);
            CameraView.texture = character.RenderTexture;
            
            _lastActiveCharacter = character;
        }
        


    }

}

