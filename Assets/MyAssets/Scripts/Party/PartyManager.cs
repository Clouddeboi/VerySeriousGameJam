using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [Header("Starting Party (up to 4)")]
    public Character[] StartingCharacters;

    [Header("Scene References")]
    public CameraShake CameraShake;
    public BuffDebuffSystem BuffDebuff;
    public FloatingTextSpawner TextSpawner;

    [Header("Party Member Prefab")]
    public GameObject PartyMemberPrefab;
    public Transform[] PartySlotTransforms;

    public List<PartyMember> ActiveMembers { get; private set; } = new List<PartyMember>();
    public static PartyManager Instance { get; private set; }

    public Health ActiveHealth => ActiveMember?.Health;
    public Transform ActiveTransform => ActiveMember?.transform;
    public PartyMember ActiveMember => ActiveMembers.Count > 0 ? ActiveMembers[0] : null;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public List<Health> AllLivingHealth()
    {
        var result = new List<Health>();
        foreach (var m in ActiveMembers)
            if (m != null && !m.Health.IsDead) result.Add(m.Health);
        return result;
    }


    public void InitializeParty()
    {
        Debug.Log($"[Party] InitializeParty called. Characters: {StartingCharacters?.Length}, Prefab: {PartyMemberPrefab}, Slots: {PartySlotTransforms?.Length}");
        ClearParty();

        if (PartyMemberPrefab == null)
        {
            Debug.LogError("[Party] PartyMemberPrefab is null — assign it in the Inspector.");
            return;
        }

        if (StartingCharacters == null || StartingCharacters.Length == 0)
        {
            Debug.LogError("[Party] StartingCharacters is empty — assign CharacterSO assets in the Inspector.");
            return;
        }

        for (int i = 0; i < StartingCharacters.Length && i < 4; i++)
        {
            if (StartingCharacters[i] == null)
            {
                Debug.LogWarning($"[Party] StartingCharacters[{i}] is null — skipping.");
                continue;
            }

            Transform slot = (PartySlotTransforms != null && i < PartySlotTransforms.Length && PartySlotTransforms[i] != null)
                ? PartySlotTransforms[i]
                : transform;

            Debug.Log($"[Party] Spawning {StartingCharacters[i].CharacterName} at slot {i} ({slot.name})");

            var go = Instantiate(PartyMemberPrefab, slot.position, Quaternion.identity, slot);
            go.name = $"PartyMember_{StartingCharacters[i].CharacterName}";

            var member = go.GetComponent<PartyMember>();
            if (member == null)
            {
                Debug.LogError($"[Party] PartyMemberPrefab is missing a PartyMember component!");
                Destroy(go);
                continue;
            }

            if (CameraShake == null) Debug.LogWarning("[Party] CameraShake is null — assign it.");
            if (BuffDebuff == null) Debug.LogWarning("[Party] BuffDebuff is null — assign it.");
            if (TextSpawner == null) Debug.LogWarning("[Party] TextSpawner is null — assign it.");

            member.Initialize(StartingCharacters[i], CameraShake, BuffDebuff, TextSpawner);
            member.Health.OnDeath += () => HandleMemberDeath(member);

            ActiveMembers.Add(member);
            Debug.Log($"[Party] Successfully added {StartingCharacters[i].CharacterName}. Party size: {ActiveMembers.Count}");
        }

        Debug.Log($"[Party] Initialization complete. {ActiveMembers.Count} members active.");
    }

    public void AddMember(Character character)
    {
        if (ActiveMembers.Count >= 4)
        {
            Debug.Log("[Party] Party is full — cannot add more members.");
            return;
        }

        int index = ActiveMembers.Count;
        Transform slot = index < PartySlotTransforms.Length
            ? PartySlotTransforms[index] : transform;

        var go = Instantiate(PartyMemberPrefab, slot.position,
            Quaternion.identity, slot);

        var member = go.GetComponent<PartyMember>();
        member.Initialize(character, CameraShake, BuffDebuff, TextSpawner);
        member.Health.OnDeath += () => HandleMemberDeath(member);

        ActiveMembers.Add(member);
        Debug.Log($"[Party] Added {character.CharacterName} to party.");
    }

    public bool IsPartyAlive()
    {
        if (ActiveMembers.Count == 0)
        {
            Debug.LogWarning("[Party] IsPartyAlive called but ActiveMembers is empty — party not yet initialized.");
            return true;
        }

        foreach (var m in ActiveMembers)
            if (m != null && !m.Health.IsDead) return true;

        return false;
    }

    public Health GetActiveHealth() => ActiveMember?.Health;

    public PartyMember GetLowestHPMember()
    {
        PartyMember lowest = null;
        int lowestHP = int.MaxValue;
        foreach (var m in ActiveMembers)
        {
            if (m == null || m.Health.IsDead) continue;
            if (m.Health.CurrentHP < lowestHP)
            {
                lowestHP = m.Health.CurrentHP;
                lowest = m;
            }
        }
        return lowest;
    }

    private void HandleMemberDeath(PartyMember member)
    {
        Debug.Log($"[Party] {member.Data.CharacterName} died.");
    }

    private void ClearParty()
    {
        foreach (var m in ActiveMembers)
            if (m != null) Destroy(m.gameObject);
        ActiveMembers.Clear();
    }
}