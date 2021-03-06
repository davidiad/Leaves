﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;
using UnityEngine.UI;

public class PaintManager : MonoBehaviour
{
    public Button onoff;
    public GameObject paintTarget;
    private Mesh mesh; // save particles in a mesh

    public ParticleSystem particleSystemTemplate;

    private bool newPaintVertices;
    private bool paintingOn;
    private Color paintColor;
    private Vector3 previousPosition;

    public List<ParticleSystem> particleSystemList; // Stores all particle systems
    public List<Vector3> currVertices; // Stores current camera positions to paint
    public ParticleSystem ps; // Stores current particle system

    [SerializeField] Camera mainCam;

    void OnEnable()
    {
        //UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;
    }

    void OnDestroy()
    {
        //UnityARSessionNativeInterface.ARFrameUpdatedEvent -= ARFrameUpdated;
    }

    // Use this for initialization
    void Start()
    {
        paintingOn = false;
        newPaintVertices = false;
        particleSystemList = new List<ParticleSystem>();
        ps = Instantiate(particleSystemTemplate);
        currVertices = new List<Vector3>();
        paintColor = Color.green;
        mesh = new Mesh();
    }

    // Update is called once per frame
    void Update()
    {
        if (paintingOn && newPaintVertices)
        {
            if (currVertices.Count > 0)
            {
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[currVertices.Count];
                int index = 0;
                foreach (Vector3 vtx in currVertices)
                {
                    particles[index].position = vtx;
                    particles[index].color = paintColor;
                    particles[index].size = 0.05f;
                    index++;
                }
                ps.SetParticles(particles, currVertices.Count);
                newPaintVertices = false;
            }
        }
        Paint();
    }

    public void TogglePaint()
    {
        paintingOn = !paintingOn;
        if (paintingOn)
        {
            onoff.transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
        } else {
            onoff.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void RandomizeColor()
    {
        if (ps.particleCount > 0)
        {
            SaveParticleSystem();
        }
        paintColor = Random.ColorHSV();
    }

    public void Reset()
    {
        foreach (ParticleSystem p in particleSystemList)
        {
            Destroy(p);
        }
        particleSystemList = new List<ParticleSystem>();

        Destroy(ps);
        ps = Instantiate(particleSystemTemplate);
        currVertices = new List<Vector3>();
    }

    private void SaveParticleSystem()
    {
        particleSystemList.Add(ps);
        ps = Instantiate(particleSystemTemplate);
        currVertices = new List<Vector3>();
    }

    public Mesh GetMesh()
    {
        // get all the particles, and save them in a mesh
        //foreach (ParticleSystem partSys in particleSystemList)
        //{
            /*
            ParticleSystem.Particle[] myParticles = (ParticleSystem)GetComponent("ParticleSystem");
            partSys.GetParticles(myParticles);
            foreach (Particle particle in myParticles)
            {

            }

            ParticleSystem.Particle[] currentParticles = new ParticleSystem.Particle[partSys.particleCount]; 
            partSys.GetParticles(currentParticles);
            foreach (Particle particle in currentParticles) 
            {
                
            }
            Vector3 [] verts = partSys.GetParticles()

        }
        */
        return mesh;
    }

    private void Paint ()
    {
        Vector3 paintPosition = paintTarget.transform.position;
            //Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane)) + Camera.main.transform.forward * 2.0f;
            //Camera.main.transform.position + Camera.main.transform.forward * 0.3f;
        if (Vector3.Distance(paintPosition, previousPosition) > 0.025f)
        {
            if (paintingOn) currVertices.Add(paintPosition);
            previousPosition = paintPosition;
            newPaintVertices = true;

        }
    }
    // Quaternion rot = Camera.main.transform.rotation;
    /*
    private void ARFrameUpdated(UnityARCamera arCamera)
    {

        Vector3 paintPosition = (Camera.main.transform.forward * 0.2f) + GetCameraPosition(arCamera);
        if (Vector3.Distance(paintPosition, previousPosition) > 0.025f)
        {
            if (paintingOn) currVertices.Add(paintPosition);
            previousPosition = paintPosition;
            newPaintVertices = true;
            Debug.Log("arCam Position: " + paintPosition);
        } else {
            Debug.Log("arCam Position, Painting off: " + paintPosition);
        }
    }
    */

    private Vector3 GetCameraPosition(UnityARCamera cam)
    {
        Matrix4x4 matrix = new Matrix4x4();
        matrix.SetColumn(3, cam.worldTransform.column3);
        return UnityARMatrixOps.GetPosition(matrix);
    }
}
