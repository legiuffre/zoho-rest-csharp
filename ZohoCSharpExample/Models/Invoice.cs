namespace ZohoCSharpExample.Models
{
    public class Owner
    {
        public string name { get; set; }
        public string id { get; set; }
        public string email { get; set; }
    }

    public class ProductName
    {
        public string Product_Code { get; set; }
        public int Qty_Ordered { get; set; }
        public string name { get; set; }
        public int Qty_in_Stock { get; set; }
        public List<object> Tax { get; set; }
        public string id { get; set; }
        public bool Taxable { get; set; }
        public object Unit_Price { get; set; }
    }

    public class InvoicedItem
    {
        public DateTime Modified_Time { get; set; }
        public string Description { get; set; }
        public int Discount { get; set; }
        public DateTime Created_Time { get; set; }
        public Owner Parent_Id { get; set; }
        public string Sequence_Number { get; set; }
        public ProductName Product_Name { get; set; }
        public int Quantity { get; set; }
        public int Tax { get; set; }
        public double Net_Total { get; set; }
        public Owner Layout { get; set; }
        public double Total { get; set; }
        public List<object> Line_Tax { get; set; }
        public object Price_Book_Name { get; set; }
        public double List_Price { get; set; }
        public string id { get; set; }
        public double Total_After_Discount { get; set; }
    }

    public class Data
    {
        public Owner Owner { get; set; }
        public string email { get; set; }
        public int Tax { get; set; }
        public List<InvoicedItem> Invoiced_Items { get; set; }
    }

    public class Invoice
    {
        public List<Data> data { get; set; }
    }

}