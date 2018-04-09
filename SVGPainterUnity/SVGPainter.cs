using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SVGPainterUnity
{
    public enum PainterState
    {
        None,
        Animating,
        Complete
    }

    public class SVGPainter : MonoBehaviour
    {
        public SVGPainterInstanciate instanciate;

        private PainterState state = PainterState.None;

        private System.Action onComplete = null;
        private System.Action onRewindComplete = null;

        // Use this for initialization
        void Start()
        {
           
        }

        // Update is called once per frame
        void Update()
        {
            if(instanciate != null){
                if (state == PainterState.Complete)
                {
                    return;
                }

                if (instanciate.painters.Count >= 1)
                {
                    int checkCompleteCount = 0;
                    for (int i = 0; i < instanciate.painters.Count; i++)
                    {
                        instanciate.painters[i].UpdateLine();
                        PainterAnimationState pstate = instanciate.painters[i].GetState();
                        if (pstate == PainterAnimationState.Complete)
                        {
                            checkCompleteCount++;
                        }
                    }
                    if (checkCompleteCount >= instanciate.painters.Count)
                    {
                        state = PainterState.Complete;

                        if (onRewindComplete != null)
                        {
                            onRewindComplete();
                            onRewindComplete = null;
                        }

                        if (onComplete != null)
                        {
                            onComplete();
                            onComplete = null;
                        }

                    }
                }
            }
        }

        public void Play(float duration = 3f, System.Func<float, float, float, float, float> _easing = null, System.Action callback = null)
        {
            state = PainterState.Animating;
            onComplete = callback;
            for (int i = 0; i < instanciate.painters.Count; i++)
            {
                instanciate.painters[i].duration = duration;
                instanciate.painters[i].Play(0f, _easing);
            }
        }

        public void Rewind(float duration = 3f, System.Func<float, float, float, float, float> _easing = null, System.Action callback = null)
        {
            state = PainterState.Animating;
            onRewindComplete = callback;
            for (int i = 0; i < instanciate.painters.Count; i++)
            {
                instanciate.painters[i].duration = duration;
                instanciate.painters[i].Rewind(0f, _easing);
            }
        }

        public bool IsActive() {
            if(instanciate != null){
                if(instanciate.painters.Count>=1){
                    return true;
                }
            }
            return false;
        }
	}
}