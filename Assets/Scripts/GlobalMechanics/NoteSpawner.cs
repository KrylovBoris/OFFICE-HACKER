// MIT License
// Copyright (c) 2020 KrylovBoris
// License information: https://github.com/KrylovBoris/OFFICE-HACKER/blob/main/LICENSE

using System;
using GlobalMechanics.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GlobalMechanics
{
    public enum SpawnDistributionType
    {
        Frame,
        UniformDistribution,
        GaussianDistribution
    }

    [Serializable]
    public struct SpawnPattern
    {
        public Transform point1;
        public Transform point2;
        public SpawnDistributionType spawnType;
    }

//Компонент, который 
    public class NoteSpawner : MonoBehaviour
    {
        public int hiddenZoneIndex = 2;
        public SpawnPattern[] spawnZones;
        public GameObject spawnableNote;
        public string[] NoteText;
    
        private void SpawnNote(string text, int chosenZone)
        {
            var spawnZone = spawnZones[chosenZone];
            Vector3 spawnPoint = Vector3.zero;
            switch (spawnZone.spawnType)
            {
                case SpawnDistributionType.Frame:
                    spawnPoint = RandomFrame(spawnZone.point1.position, spawnZone.point2.position);
                    break;
                case SpawnDistributionType.UniformDistribution:
                    spawnPoint = RandomUniform(spawnZone.point1.position, spawnZone.point2.position);
                    break;
                case SpawnDistributionType.GaussianDistribution:
                    spawnPoint = RandomGaussian(spawnZone.point1.position, spawnZone.point2.position);
                    break;
            }

            var noteGo = Instantiate(spawnableNote, spawnPoint, spawnZone.point1.rotation);
            var noteComponent = noteGo.GetComponent<Note>();
            Debug.Log("Point: " + spawnPoint + " at zone" + chosenZone);
            noteComponent.SetText(text);
        }

        private Vector3 RandomGaussian(Vector3 point1, Vector3 point2)
        {
            //Making Gaussian distribution
            float r, phi, theta;
            Vector3 randomPoint;
            do
            {
                r = Mathf.Sqrt(-2 * Mathf.Log(Random.Range(0.0f, 1.0f)));
                phi = 2 * Mathf.PI * Random.Range(0.0f, 1.0f); 
                theta = 2 * Mathf.PI * Random.Range(0.0f, 1.0f);
                randomPoint = new Vector3(r * Mathf.Sin(theta) * Mathf.Cos(phi), r * Mathf.Sin(theta) * Mathf.Sin(phi), r * Mathf.Cos(theta));
            } while (randomPoint.magnitude > 1);

            float xScale = 0.5f * Mathf.Abs(point2.x - point1.x), 
                yScale = 0.5f * Mathf.Abs(point2.y - point1.y), 
                zScale = 0.5f * Mathf.Abs(point2.z - point1.z);
        
            randomPoint = new Vector3(xScale * randomPoint.x, yScale * randomPoint.y, zScale * randomPoint.z);
        
            var centerPoint = point1 + 0.5f * (point2 - point1);       

            Vector3 point = centerPoint + randomPoint;
            return point;
        }

        private Vector3 RandomUniform(Vector3 point1, Vector3 point2)
        {
            return new Vector3(Random.Range(point2.x, point1.x), Random.Range(point2.y, point1.y), Random.Range(point2.z, point1.z) );
        }

        private Vector3 RandomFrame(Vector3 point1, Vector3 point2)
        {
            var coin = Random.Range(0.0f, 1.0f);
            float x, y, z;
            z = point2.z;
            if (coin > 0.5)
            {
                coin = Random.Range(0.0f, 1.0f);
                y = (coin > 0.5) ? point1.y : point2.y;
                x = Random.Range(point2.x, point1.x);
            }
            else
            {
                coin = Random.Range(0.0f, 1.0f);
                x = (coin > 0.5) ? point1.x : point2.x;
                y = Random.Range(point2.y, point1.y);
            }
            return new Vector3(x,y,z);
        }

        public void SpawnManyNotes(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                SpawnNote("Test" + i, Random.Range(0, spawnZones.Length - 1));
            }
        }

        public void SpawnHiddenNote(string text)
        {
            SpawnNote(text, hiddenZoneIndex);
        }
    }
}