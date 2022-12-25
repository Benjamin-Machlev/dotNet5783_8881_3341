﻿using BLApi;
using PL.Product;
using System.Windows;

namespace PL.Cart;

/// <summary>
/// Interaction logic for Cart.xaml
/// </summary>
public partial class Cart : Window
{
    private BO.Cart _cart;
    private BLApi.IBl? _bl = BLApi.Factory.Get();
    public Cart(BO.Cart cart)
    {
        InitializeComponent();
        _cart = cart;
        ItemsList.ItemsSource = cart.ItemsList;
        CustomerName.Text = cart.CustomerName;
        CustomerEmail.Text = cart.CustomerEmail;
        CustomerAddress.Text = cart.CustomerAddress;
        TotalPriceInCart.Text = cart.TotalPriceInCart.ToString();
    }

    private void ItemsList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (IsMouseCaptureWithin)
            new ProductView(_bl, ((BO.OrderItem)ItemsList.SelectedItem).ProductID, _cart, "updateCart").Show();
        this.Close();
    }

    private void CheckOut_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _bl.Cart.ConfirmedOrder(_cart);
            MessageBox.Show("SUCCSES", "SUCCSES", MessageBoxButton.OK, MessageBoxImage.Information);
            //ASK???
            Application.Current.Shutdown();
        } catch (System.Exception)
        {

            throw;
        }
    }
}