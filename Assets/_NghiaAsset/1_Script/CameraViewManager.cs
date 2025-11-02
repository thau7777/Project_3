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
        
        private void Start()
        {
        }


        public void SetCameraView(Character character)
        {
            if (character == null || character.RenderTexture == null)
            {
                CameraView.texture = null;
                return;
            }

            CameraView.texture = character.RenderTexture;
        }
        


    }

}

