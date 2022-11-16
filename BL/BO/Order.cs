﻿namespace BO;

public class Order
{
    public int OrderID { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerAddress { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime ShipDate { get; set; }
    public DateTime DeliveryDate { get; set; }
    public OrderItem? ItemsList { get; set; }
    public double TotalPrice { get; set; }

    public override string ToString() => $"Customer Name: {CustomerName}\n" +
                                         $"Customer Email: {CustomerEmail}\n" +
                                         $"Customer Address: {CustomerAddress}\n" +
                                         $"Order Date: {OrderDate}\n" +
                                         $"Status: {Status}\n" +
                                         $"ShipDate: {ShipDate}\n" +
                                         $"Delivery Date: {DeliveryDate}\n" +
                                         $"Items List: {ItemsList}\n" +
                                         $"Total Price: {TotalPrice}\n";

}