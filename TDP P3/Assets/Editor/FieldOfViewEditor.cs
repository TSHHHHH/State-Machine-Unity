using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
  private void OnSceneGUI()
  {
    FieldOfView fov = (FieldOfView)target;

    Handles.color = Color.white;

    // Draw a wire arc in 2D space
    Handles.DrawWireArc(fov.transform.position, Vector3.forward, Vector3.right, 360, fov.enemyStats.viewDistance);

    // Draw another wire arc in 2D space for the close view radius
    Handles.color = Color.yellow;

    // Compute the center of the close view radius
    Vector3 closeViewCenter = fov.transform.position + fov.transform.up * fov.enemyStats.closeViewDistance;

    float closeViewRadius = fov.enemyStats.closeViewDistance + fov.enemyStats.closeViewPuffSize;
    Handles.DrawWireArc(closeViewCenter, Vector3.forward, Vector3.right, 360, closeViewRadius);

    Vector3 viewAngleA = fov.DirFromAngle(-fov.enemyStats.viewAngle / 2, false);
    Vector3 viewAngleB = fov.DirFromAngle(fov.enemyStats.viewAngle / 2, false);

    // Draw lines to the view angles
    Handles.color = Color.yellow;
    Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.enemyStats.viewDistance);
    Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.enemyStats.viewDistance);

    // Draw lines to the visible targets
    Handles.color = Color.red;
    foreach (Transform visibleTarget in fov.visibleTargets)
    {
      Handles.DrawLine(fov.transform.position, visibleTarget.position);
    }

    //// Draw lines to the visible posts
    //Handles.color = Color.green;
    //foreach (Transform visiblePost in fov.visibleCovers)
    //{
    //  Handles.DrawLine(fov.transform.position, visiblePost.position);
    //}
  }
}
