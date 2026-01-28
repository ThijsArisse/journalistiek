using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Whisper;

public class RandomObjectiveGenerator : MonoBehaviour
{
    [System.Serializable] 
    public class Objective
    {
        public string name;
        public string description;
        public string prompt;
        public float weight;
    }

    [SerializeField] private TextMeshProUGUI objectiveNameText;
    [SerializeField] private TextMeshProUGUI objectiveNameDescriptionText;
    [SerializeField] private List<Objective> objectives;

    [SerializeField] private WhisperManager whisperManager;

    private Objective currentObjective;

    private void Start()
    {
        currentObjective = GetRandomObjective(CalculateWeights());
        DisplayObjective(currentObjective);
        whisperManager.initialPrompt = currentObjective.prompt;
    }

    private Objective GetRandomObjective(float totalWeight)
    {
        float randomNumb = Random.Range(0f, totalWeight);

        foreach (Objective objective in objectives)
        {
            if (objective.weight > randomNumb)
            {
                return objective;
            }
            else
            {
                randomNumb -= objective.weight;
            }
        }

        return objectives[0];
    }

    private float CalculateWeights()
    {
        float weight = 0f;
        
        foreach (Objective objective in objectives)
        {
            weight += objective.weight;
        }

        return weight;
    }

    private void DisplayObjective(Objective currentObjective)
    {
        objectiveNameText.text = currentObjective.name;
        objectiveNameDescriptionText.text = currentObjective.description;
    }
}
