using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    public float gap = 70;
    private float containerWidth;
    private float cotainerHeight;
    private List<RowContainer> rowContainer = new List<RowContainer>();

    private void Awake()
    {
        RectTransform rt = (RectTransform)gameObject.transform;
        containerWidth = rt.rect.width * rt.localScale.x;
        cotainerHeight = rt.rect.height * rt.localScale.y;

        rt.position += new Vector3(100, 0, 0);
    }

    void centerChildren()
    {
        // child.gameObject.transform.localPosition = new Vector3(0,0,0);
        float x0;
        rowContainer.ForEach((RowContainer row) => {
            x0 = 0;
            if (row.rowItems.Count > 1)
            {
                x0 = -row.Width / 2;
            }

            for (int i = 0; i < row.rowItems.Count; i++)
            {
                RectTransform rt = (RectTransform)row.rowItems[i].gameObject.transform;
                if (row.rowItems.Count > 1)
                {
                    x0 = x0 + (rt.rect.width * rt.localScale.x) / 2;
                }
                row.rowItems[i].gameObject.transform.localPosition = new Vector3(x0, row.y, 0);
                x0 += (rt.rect.width * rt.localScale.x) / 2 + gap;
            }
        });
    }

    // Determine number of rows
    void calcRowCount()
    {

        RowContainer row = new RowContainer();
        float totalWidth = 0;
        RectTransform child;
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            child = (RectTransform)this.gameObject.transform.GetChild(i);
            if (!child.gameObject.activeSelf)
            {
                continue;
            }

            if (row.Height < (child.rect.height * child.localScale.y))
            {
                row.Height = child.rect.height * child.localScale.y;
            }

            totalWidth +=(child.rect.width * child.localScale.x);
            if (totalWidth > containerWidth)
            {
                rowContainer.Add(row);
                row = new RowContainer();
                row.Height = child.rect.height * child.localScale.y;
                totalWidth = (child.rect.width * child.localScale.x);
            }

            row.Width = totalWidth;
            row.rowItems.Add(child.gameObject);
        }
        RectTransform rt = (RectTransform)gameObject.transform;
        rt.position += new Vector3(containerWidth/2- row.Width/2, 0, 0);


        if (row.rowItems.Count > 0)
        {
            rowContainer.Add(row);
        }

        //  Calculate row y pos, center = 0
        float rowHeight = 0;
        rowContainer.ForEach((RowContainer row) => {
            rowHeight += row.Height;
        });
        rowHeight += (rowContainer.Count - 1) * gap;

        if (rowContainer.Count > 1)
        {
            float y0 = rowHeight / 2;
            for (int i = 0; i < rowContainer.Count; i++)
            {
                rowContainer[i].y = y0 - rowContainer[i].Height / 2;
                y0 = y0 - rowContainer[i].Height + gap;
            }
        }

    }

   

    void Start()
    {
        calcRowCount();
        centerChildren();
    }

    // Update is called once per frame
    void Update()
    {
        

    }
}

class RowContainer
{
    public List<GameObject> rowItems = new List<GameObject>();
    public float Height = 0;
    public float Width = 0;

    public float x;
    public float y;
}

