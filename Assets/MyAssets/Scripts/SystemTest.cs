using UnityEngine;

public class SystemTest : MonoBehaviour
{
    public SlotMachineSystem slotMachine;
    public AttackBuilder attackBuilder;
    public DamageSystem damageSystem;
    public Health dummyEnemy;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RunOneAttack();
        }
    }

    private void RunOneAttack()
    {
        SlotResult result = slotMachine.Spin();
        Attack attack = attackBuilder.Build(result);
        damageSystem.Resolve(attack, dummyEnemy);
    }
}