[System.Serializable]
public class AbilityData
{
    public string name;
    public bool targeted;
    public float manaCost;
    public int cooldown;
    public bool active;
    public string icon;     // path to icon
    public System.Collections.Generic.List<AbilityProperties> properties = new System.Collections.Generic.List<AbilityProperties>();
    public System.Collections.Generic.List<AbilityChargesConditions> chargesConditions = new System.Collections.Generic.List<AbilityChargesConditions>();

    public float GetProperty(string name)
    {
        return properties.Find(x => x.type == name).value;
    }
    public bool GetChargeCondition(string name)
    {
        if (chargesConditions.Count == 0) return false;
        AbilityChargesConditions atc = chargesConditions.Find(x => x.condition == name);//.condition;
        return atc != null && atc.condition != "";
    }
}
[System.Serializable]
public class AbilityDataList
{
    public System.Collections.Generic.List<AbilityData> abilities;
}
[System.Serializable]
public class AbilityProperties
{
    public string type;
    public float value;
}
[System.Serializable]
public class AbilityPropertiesList
{
    public System.Collections.Generic.List<AbilityData> properties;
}
[System.Serializable]
public class AbilityChargesConditions
{
    public string condition;
}
[System.Serializable]
public class AbilityChargesConditionsList
{
    public System.Collections.Generic.List<AbilityData> chargesConditions;
}