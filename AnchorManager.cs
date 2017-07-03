
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR.WSA.Persistence;
using UnityEngine.VR.WSA;
using HoloToolkit.Unity.SpatialMapping;
using System;
using System.Linq;
using UnityEngine.Events;
using System.Collections;

namespace HoloToolkit.Unity
{

    public class AnchorManager : MonoBehaviour
    {
        public string anchorName;


        public AnchorMover AnchorMover;

        public bool encode_rotation;
        public bool encode_scale;

        private WorldAnchorStore AnchorStore;
        private string rotation_key = "_rotation";
        private string scale_key = "_rotation";

        void Awake()
        {
            Debug.Log("AnchorStore awake");

            AnchorStore = null;
            WorldAnchorStore.GetAsync(AnchorStoreReady);
        }

        public void AnchorStoreReady(WorldAnchorStore anchorStore)
        {
            Debug.Log("AnchorStore returned");
            AnchorStore = anchorStore;
            Load();
        }

        public void CenterOnMoverPlaceholder()
        {
            Overwrite(AnchorMover.Placeholder.transform.position);
        }
       


        //Remove anchor but do not delete from anchor store
        public void Remove()
        {
            Debug.Log("Remove Anchor on " + gameObject.name);

            DestroyImmediate(GetComponent<WorldAnchor>());
        }

        //Remove anchor but do not delete from anchor store
        public void Delete()
        {
            Debug.Log("Request Delete Anchor " + anchorName);
            if (AnchorStore == null) { Debug.Log("AnchorStore is Null"); return; }

            var ids = AnchorStore.GetAllIds();

            for (int i = 0; i < ids.Length; i++)
            {
                var anchorTitle = ids[i];
                Debug.Log(anchorTitle);

                if (anchorTitle == anchorName)
                {
                    Debug.Log("Deleting");
                    AnchorStore.Delete(anchorTitle);
                }
            }
        }

        //Upsert should create/update the anchor of name anchorName in the world anchor store to the location of the obj
        //Upsert should create/update the 1 anchor attached to obj
        //
        internal void Load()
        {
            Debug.Log("Request Load Anchor " + anchorName + " : AnchorStore Loaded: " + (AnchorStore != null));

            if (AnchorStore == null) { Debug.Log("AnchorStore is Null"); return; }

            var ids = AnchorStore.GetAllIds();
            for (int i = 0; i < ids.Length; i++)
            {
                var anchorTitle = ids[i];
                if (anchorTitle == anchorName)
                {
                    AnchorStore.Load(anchorTitle, gameObject);
                }
            }

            if (encode_rotation)
            {
                var rotation_transform = new GameObject();
                for (int i = 0; i < ids.Length; i++)
                {
                    var anchorTitle = ids[i];
                    if (anchorTitle == anchorName + rotation_key)
                    {
                        AnchorStore.Load(anchorTitle, rotation_transform);
                        transform.LookAt(rotation_transform.transform.position);
                    }
                }
            }

            if (encode_scale)
            {
                var scale_transform = new GameObject();
                for (int i = 0; i < ids.Length; i++)
                {
                    var anchorTitle = ids[i];
                    if (anchorTitle == anchorName + scale_key)
                    {
                        AnchorStore.Load(anchorTitle, scale_transform);
                    }
                }
            }
        }
       

        //Upsert should create/update the anchor of name anchorName in the world anchor store to the location of the obj
        //Upsert should create/update the 1 anchor attached to obj
        //
        internal void Create()
        {

            Debug.Log("Request Create Anchor " + anchorName + " : AnchorStore Loaded: " + (AnchorStore != null));

            if (AnchorStore == null) { Debug.Log("AnchorStore is Null"); return; }
            Debug.Log("Creating Anchor " + anchorName);
            var worldAnchor = gameObject.AddComponent<WorldAnchor>();

            if (worldAnchor.isLocated)
            {
                Debug.Log("Saving persisted position immediately");
                bool saved = AnchorStore.Save(anchorName, worldAnchor);
                Debug.Log("saved: " + saved);
            }
            else
            {
                Debug.Log("Setting a listener for main anchor");
                worldAnchor.OnTrackingChanged += AttachingAnchor_OnTrackingChanged;
            }

            if (encode_rotation)
            {
                var rotation_transform = new GameObject();
                rotation_transform.transform.position = transform.position + transform.forward;
                var rotation_anchor = rotation_transform.AddComponent<WorldAnchor>();

                if (rotation_anchor.isLocated)
                {
                    Debug.Log("Saving persisted position immediately");
                    bool saved = AnchorStore.Save(anchorName + rotation_key, rotation_anchor);
                    Debug.Log("saved rotation anchor: " + saved);
                }
                else
                {
                    Debug.Log("Setting a listener for rotation anchor");
                    rotation_anchor.OnTrackingChanged += AttachingRotationAnchor_OnTrackingChanged;
                }
            }

            if (encode_scale)
            {
                var scale_transform = new GameObject();
                scale_transform.transform.position = transform.position + (transform.forward * transform.localScale.z) + (transform.right * transform.localScale.x) + (transform.up * transform.localScale.y);
                var scale_anchor = scale_transform.AddComponent<WorldAnchor>();

                if (scale_anchor.isLocated)
                {
                    Debug.Log("Saving persisted position immediately");
                    bool saved = AnchorStore.Save(anchorName + scale_key, scale_anchor);
                    Debug.Log("saved scale anchor: " + saved);
                }
                else
                {
                    Debug.Log("Setting a listener for scale anchor");
                    scale_anchor.OnTrackingChanged += AttachingScaleAnchor_OnTrackingChanged;
                }
            }
        }

        public void AttachingAnchor_OnTrackingChanged(WorldAnchor self, bool located)
        {
            Debug.Log("AttachingAnchor_OnTrackingChanged");
            if (located)
            {
                Debug.Log("Saving persisted position in callback");
                bool saved = AnchorStore.Save(anchorName, self);
                Debug.Log("saved: " + saved);
                self.OnTrackingChanged -= AttachingAnchor_OnTrackingChanged;
            }
        }

        public void AttachingRotationAnchor_OnTrackingChanged(WorldAnchor self, bool located)
        {
            Debug.Log("AttachingRotationAnchor_OnTrackingChanged");
            if (located)
            {
                Debug.Log("Saving persisted position in callback");
                bool saved = AnchorStore.Save(anchorName + rotation_key, self);
                Debug.Log("saved: " + saved);
                self.OnTrackingChanged -= AttachingRotationAnchor_OnTrackingChanged;
            }
        }

        public void AttachingScaleAnchor_OnTrackingChanged(WorldAnchor self, bool located)
        {
            Debug.Log("AttachingScaleAnchor_OnTrackingChanged");
            if (located)
            {
                Debug.Log("Saving persisted position in callback");
                bool saved = AnchorStore.Save(anchorName + scale_key, self);
                Debug.Log("saved: " + saved);
                self.OnTrackingChanged -= AttachingScaleAnchor_OnTrackingChanged;
            }
        }

        //Upsert should create/update the anchor of name anchorName in the world anchor store to the location of the obj
        //Upsert should create/update the 1 anchor attached to obj
        //
        internal void Overwrite(Vector3 desiredLocation)
        {

            Debug.Log("Request Overwrite Anchor " + anchorName + " : AnchorStore Loaded: " + (AnchorStore != null));

            if (AnchorStore == null) { Debug.Log("AnchorStore is Null"); return; }
            Delete();
            Remove();
            gameObject.transform.position = desiredLocation;
            if(encode_rotation && AnchorMover)
            {
                transform.rotation = AnchorMover.Placeholder.transform.rotation;
            }
            Create();

            
        }
    }
}
