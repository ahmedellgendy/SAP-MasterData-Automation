namespace MasterDataAutomation.Application.Common.Helpers;

public static class BranchNormalizer
{
    public static string Normalize(string branch)
    {
        if (string.IsNullOrWhiteSpace(branch))
            return string.Empty;

        branch = branch.Trim().ToLower();

        return branch switch
        {
            // 10th
            "العاشر" => "10th",
            "10th" => "10th",
            "10Th" => "10th",
            "10TH" => "10th",

            // Giza
            "الجيزة" => "Giza",
            "الجيزه" => "Giza",
            "giza" => "Giza",
            "GIZA" => "Giza",
            "Giza" => "Giza",

            // Alexandria
            "الإسكندرية" => "Alex",
            "الاسكندرية" => "Alex",
            "الأسكندرية" => "Alex",
            "الإسكندريه" => "Alex",
            "الاسكندريه" => "Alex",
            "الأسكندريه" => "Alex",
            "alex" => "Alex",
            "Alex" => "Alex",
            "ALEX" => "Alex",
            "alexandria" => "Alex",
            "Alexandria" => "Alex",

            // Mansoura
            "المنصورة" => "Mansora",
            "المنصوره" => "Mansora",
            "mansoura" => "Mansora",
            "Mansoura" => "Mansora",
            "mansora" => "Mansora",
            "Mansora" => "Mansora",

            _ => branch
        };
    }
}