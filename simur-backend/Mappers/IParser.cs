namespace rest_with_asp_net_10_cviana.Data.Converter.Contract
{
    public interface IParser<O, D>
    {
        public D Parse(O origin);

        public List<D> ParseList(List<O> origin);
    }
}
