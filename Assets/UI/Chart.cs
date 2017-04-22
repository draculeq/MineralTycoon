using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.UI
{
    public class Chart : MonoBehaviour
    {
        private Image image;

        int width = 60;
        int height = 100;

        void Start()
        {
            image = GetComponent<Image>();
            StartCoroutine(Up());
        }
        

        IEnumerator Up()
        {
            while (true)
            {
                image.sprite = Sprite.Create(GetChart(Sinus), new Rect(0, 0, width, height), Vector2.zero);
                //yield return new WaitForSeconds(1);
                yield break;
            }
        }
        private double Sinus(DateTime x)
        {
            var time = ((int)(x - DateTime.Today).TotalSeconds);
            var value = (float)(time / 10f);

            return
                (double)Math.Sin(
                (Math.Sin(value * 2f) * 0.3f
                + Mathf.Sin(value) * 0.3f
                + Math.Sin(value * 5f) * 0.2f
                + Math.Sin(value * 1.4f) * 0.2f
                + Math.Sin(value * 0.8f) * 0.4f)
                * 0.4f
                + (Math.Sin(value * 0.85f) * 0.2f
                + Math.Sin(value * 0.2f) * 0.2f
                + Math.Sin(value * 0.3f) * 0.3f)
                + (Math.Sin(value * 0.03f)
                + Math.Sin(value * 0.035f)
                + Math.Sin(value * 0.05f)) * 0.4f)
                ;
        }

        private Texture2D GetChart(Func<DateTime, double> function)
        {
            var tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            var colors = new Color[width * height];
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    if (y / (float)height * 2 - 1f > function(DateTime.UtcNow.Subtract(new TimeSpan(0,0,x))))
                        colors[x + y * width] = Color.black;
                    else
                        colors[x + y * width] = Color.white;
                }
            tex.SetPixels(colors);
            tex.Apply();
            return tex;
        }
    }
}
