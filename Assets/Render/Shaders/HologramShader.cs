using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramShader : MonoBehaviour
{

    Material material;
    AudioSource audioSource;
    float time;
    float glitchSpeed;
    private bool active;
    public float randomTimeFactor = 2.0f;
    private void Awake() {
        material = GetComponent<Renderer>().material;
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        activate();
    }

    [ContextMenu("Activate")]
    public void activate() {
        active = true;
        StartCoroutine("main");
    }

    [ContextMenu("Desactivate")]
    public void desactivate() {
        active = false;
    }

    IEnumerator main() {

        while (active) {
            material.SetInt("_DoGlitch", 0);

            time = material.GetFloat("_GlitchTime");
            time += Random.Range(-1.0f, 0.5f) * randomTimeFactor;
            yield return new WaitForSeconds(time);

            //audioSource.Play(); // HACK encontrar un sonido mejor, o quitarselo sin más.
            yield return new WaitForSeconds(0.05f); //HACK el sonido sueno un poquito más tarde. Cuando quitemos el placeholder igual se arreglas
            material.SetFloat("_CurrentTime", Time.time);
            material.SetInt("_DoGlitch", 1);

            glitchSpeed = material.GetFloat("_GlitchSpeed");
            yield return new WaitForSeconds(1 / glitchSpeed);
        }
    }
}
