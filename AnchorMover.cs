// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA;
using System.Collections;
using UnityEngine.Events;

namespace HoloToolkit.Unity.SpatialMapping
{
    //The city placer acts as the main controller for all placement needs of the city.
    //It should coordinate control between itself and the target-tracker, as well as read/write anchor data for the rig.
    public class AnchorMover : MonoBehaviour, IInputClickHandler
    {
        //Outwardly accessible state
        public bool IsBeingPlaced;
        public UnityEvent OnPositionSet;

        //Mover internals
        public GameObject Placeholder;


        public Vector3 FinalOrientation;
        public float PlacementFlexibility;

        public float MinDistanceFromTarget;

        //Privates
        protected SpatialMappingManager spatialMappingManager;
        private Vector3 default_placeholder_scale;

        //Set Private internals
        protected virtual void Start()
        {
            spatialMappingManager = SpatialMappingManager.Instance;
            default_placeholder_scale = Placeholder.transform.localScale;
        }

        //Smoothly reposition the thing
        protected virtual void FixedUpdate()
        {
            //Get gaze targeting info
            Vector3 headPosition = Camera.main.transform.position;
            Vector3 gazeDirection = Camera.main.transform.forward;
            RaycastHit hitInfo;

            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo, 30.0f, spatialMappingManager.LayerMask))
            {
                var distance_from_target = Vector3.Distance(transform.position, hitInfo.point);
                var degrees_away_from_final_orientation = Quaternion.Angle(Quaternion.Euler(hitInfo.normal), Quaternion.Euler(FinalOrientation));

                //If gaze is on a wall suitable surface and far enough from the target object, move the placeholder there
                if (distance_from_target > MinDistanceFromTarget && degrees_away_from_final_orientation < PlacementFlexibility)
                {
                    IsBeingPlaced = true;
                    Placeholder.SetActive(true);

                    Placeholder.transform.localScale = default_placeholder_scale * (1 - (Vector3.Distance(Placeholder.transform.position, hitInfo.point)));
                    Placeholder.transform.position = Vector3.Lerp(Placeholder.transform.position, hitInfo.point, 0.05f);
                }
                else
                {
                    Placeholder.transform.position = hitInfo.point;
                    IsBeingPlaced = false;
                    Placeholder.SetActive(false);
                }
            }
        }

        public virtual void OnInputClicked(InputClickedEventData eventData)
        {
            OnInputClicked();
        }
        public virtual void OnInputClicked()
        {
            if (!IsBeingPlaced) return;
            OnPositionSet.Invoke();
        }
    }
}
