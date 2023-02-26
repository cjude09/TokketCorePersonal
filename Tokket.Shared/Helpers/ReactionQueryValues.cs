public class ReactionQueryValues
{
    public int limit = 20;
    public string kind = "";
    public string item_id = "";
    public string activity_id = "";
    public string user_id = "";
    public string reaction_id = "";
    public string pagination_id = "";
    public int? reaction_total = null;
    public int? detail_number = 0; //Less than 0: All reactions, 0: Tok Level, 1+: Detail Level
    public bool? user_likes { get; set; } //default: false
    /// <summary> Used for user_likes. Different from user_id, which filters out other users </summary>
    public string userid = "";
    /// <summary> Used for user_likes</summary>
    public string token = "";
}