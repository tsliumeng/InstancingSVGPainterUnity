using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SVGPainterUnity;

public class Demo : MonoBehaviour {

    public GameObject cam;
    public List<SVGPainter> painters = new List<SVGPainter>();

    public GameObject btn;

    private Vector3 offsetPos = new Vector3(0f, 0f, -0.21f);
    private int playCount = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (playCount >= painters.Count){
            return;
        }

        Vector3 diff = (painters[playCount].gameObject.transform.position+offsetPos) - cam.transform.position;
        Vector3 v = diff * 0.1f;
        cam.transform.position += v;
	}

    private void PlayNext() {
        painters[playCount].Play(3f, PainterEasing.EaseInOutCubic, () => {

            if (playCount >= painters.Count-1) {
                return;
            }
            painters[playCount].Rewind(3f, PainterEasing.EaseInOutCubic, () => {

                playCount++;
                if (playCount >= painters.Count) {
                    return;
                }

                PlayNext();

            });
        }); 
    }

    public void OnPlay()
    {
        PlayNext();
        btn.SetActive(false);
    }
}
