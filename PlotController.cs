using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//This script will take the Peak Point, Threshold, and Flux point from the script Spectral Flux Analyzer (SFA) to create the platforms in the scene.
//This code used snippets from another project, but was heavly modified to meet my purposes
//This script will convert the three (really 2) values into a single point in space that platforms are drawn to



public class PlotController : MonoBehaviour
{
    public List<Transform> plotPoints; // locations of the plots
    private Material highlightMaterial;
    public int displayWindowSize = 300; // width of the window


    // I first run through the points in the list from the SFA to create the points for the platforms to grow from
    void Start()
    {
        plotPoints = new List<Transform>();

        float localWidth = transform.Find("Point/BasePoint").localScale.x;

        // -n/2...0...n/2
        for (int i = 0; i < displayWindowSize; i++)
        {
            //Instantiate point
            Transform t = (Instantiate(Resources.Load("Point"), transform) as GameObject).transform;

            // Set position
            float pointX = (displayWindowSize * 2) * -1 * localWidth + i * localWidth;
            t.localPosition = new Vector3(pointX, t.localPosition.y, t.localPosition.z);

            plotPoints.Add(t);
        }
    }

    //as the viewing window pans right, the different points placed earlier are updated.
    public void updatePlot(List<SpectralFluxInfo> pointInfo, int curIndex = -1)
    {
        if (plotPoints.Count < displayWindowSize - 1)
            return;

        int numPlotted = 0;
        int windowStart = 0;
        int windowEnd = 0;

        if (curIndex > 0) //determines which points are in the view and give them the visual characteristics they need
        {
            windowStart = Mathf.Max(0, curIndex - displayWindowSize / 2);
            windowEnd = Mathf.Min(curIndex + displayWindowSize / 2, pointInfo.Count - 1);
        }
        else 
        {
            windowStart = Mathf.Max(0, pointInfo.Count - displayWindowSize - 1);
            windowEnd = Mathf.Min(windowStart + displayWindowSize, pointInfo.Count);
        }

        for (int i = windowStart; i < windowEnd; i++)
        {
            int plotIndex = numPlotted;
            numPlotted++;

            Transform fluxPoint = plotPoints[plotIndex].Find("FluxPoint"); //acts as noise which widens the bars (so not used, but left here incase I can find use later)
            Transform threshPoint = plotPoints[plotIndex].Find("ThreshPoint"); // maximum height wont go out of window
            Transform peakPoint = plotPoints[plotIndex].Find("PeakPoint"); //the height of all bars


            if (pointInfo[i].isPeak) // plot the peak value bars/ we only use this as our platforms
            {
                float height = pointInfo[i].spectralFlux; // gets the height from the peak points bars

                setPointHeight(peakPoint, height);
                setPointHeight(fluxPoint, 0f); //manually removing flux
            }
            else // flux point makes a trail if this isn't here
            {
               setPointHeight(fluxPoint, pointInfo[i].spectralFlux);
                setPointHeight(peakPoint, 0f); // make peakpoints height to zero if its not the pick point 
            }

           setPointHeight(threshPoint, pointInfo[i].threshold);
        }
    }

    //the main function that actually sets the height
    public void setPointHeight(Transform point, float height) 
    {
        float displayMultiplier = 0.06f;

        Vector3 scale = new Vector3(0.4f, height * 1.1f, point.localScale.z);
        point.DOScale(scale, 0.01f);
        //point.localScale = scale;
    }
}