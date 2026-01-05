using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlockExplosion : MonoBehaviour
{
    [Header("Uncommon Ice Block Materials")]
    [SerializeField] Material uncommonSparksMat;

    [Header("Common Ice Block Materials")]
    [SerializeField] Material commonSparksMat;

    [Header("Rare Ice Block Materials")]
    [SerializeField] Material rareSparksMat;

    [SerializeField] ParticleSystem sparksParticleSystem;
    [SerializeField] ParticleSystem sphereParticleSystem;

    public Gradient sphereUncommonParticlesGradient;
    public Gradient sphereCommonParticlesGradient;
    public Gradient sphereRareParticlesGradient;

    private float speed;
    private GameMaster gameMaster;

    private void Start()
    {
        gameMaster = GameMaster.Instance;
        speed = gameMaster.globalObstacleSpeed;
    }

    private void Update()
    {
        transform.Translate(0, 0, -1 * speed * Time.deltaTime);
    }

    public void SetParticleMaterials(int rarity)
    {
        //GetParticleSystems();
        var sparksRenderer = sparksParticleSystem.GetComponent<ParticleSystemRenderer>();
        var col = sphereParticleSystem.colorOverLifetime;

        /* 1 = common, 2 = uncommon, 3 = Rare */
        switch (rarity)
        {
            case 1:
                sparksRenderer.material = commonSparksMat;
                sparksRenderer.trailMaterial = commonSparksMat;
                col.color = sphereCommonParticlesGradient;
                break;
            case 2:
                sparksRenderer.material = uncommonSparksMat;
                sparksRenderer.trailMaterial = uncommonSparksMat;
                col.color = sphereUncommonParticlesGradient;
                break;
            case 3:
                sparksRenderer.material = rareSparksMat;
                sparksRenderer.trailMaterial = rareSparksMat;
                col.color = sphereRareParticlesGradient;
                break;
        }

        sparksParticleSystem.Play();
        sphereParticleSystem.Play();
        Destroy(gameObject, 1.5f);
    }

  
}
