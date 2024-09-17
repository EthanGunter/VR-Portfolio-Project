public interface IAbilityComponent
{
    public AbilityData Ability { get; }
    
    public void InitializeAbilityData(AbilityData ability);
}