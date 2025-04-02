public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        if (context.Recipes.Any()) return;

        var eggs = new Ingredient { Name = "munad" };
        var milk = new Ingredient { Name = "piim" };

        var recipes = new List<Recipe>
        {
            new Recipe { Name = "Omlett", Ingredients = new List<Ingredient> { eggs, milk } }
        };

        context.Recipes.AddRange(recipes);
        context.SaveChanges();
    }
}
