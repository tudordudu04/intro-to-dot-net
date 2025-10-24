namespace ProiectIndividual.Exceptions;

public class ProductNotFoundException : BaseException
{
    public List<string> Errors { get; } 
    
    protected ProductNotFoundException(IEnumerable<string> errors) : 
        base("Product not found.", 404, "PRODUCT_NOT_FOUND_ERROR")
    {
        Errors = errors.ToList();
    }

    protected ProductNotFoundException(string error) : base("Product not found.", 404, "PRODUCT_NOT_FOUND_ERROR")
    {
        Errors = [error];
    }
}