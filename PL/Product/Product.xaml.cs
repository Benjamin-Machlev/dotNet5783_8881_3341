﻿using BO;
using System;
using System.Windows;

namespace PL.Product;

/// <summary>
/// Interaction logic for Product.xaml
/// </summary>
public partial class ProductView : Window
{
    /// <summary>
    /// Access for the logical layer
    /// </summary>
    private BLApi.IBl? _bl;
    private BO.Cart? _cart;
    private BO.Product? product;
    /// <summary>
    /// Constructor for a window to add a product
    /// </summary>
    /// <param name="bl"></param>
    public ProductView(BLApi.IBl? bl)
    {
        InitializeComponent();
        _bl = bl;
        Catgory.ItemsSource = Enum.GetValues(typeof(BO.Category));
        UpdateProduct.Visibility = Visibility.Hidden;
        AddToCart.Visibility = Visibility.Hidden;
        UpdateCart.Visibility = Visibility.Hidden;
        UpdateAmountTB.Visibility = Visibility.Hidden;
        UpdateAmountL.Visibility = Visibility.Hidden;
    }

    /// <summary>
    /// Constructor for a window to update a product
    /// </summary>
    /// <param name="bl"></param>
    public ProductView(BLApi.IBl? bl, int updateProductID)
    {
        InitializeComponent();
        _bl = bl;
        Catgory.ItemsSource = Enum.GetValues(typeof(BO.Category));
        product = _bl?.Product.GetProductManger(updateProductID)!;
        DataContext = product;
        Id.IsEnabled = false;
        Catgory.SelectedItem = product.Category;
        AddProduct.Visibility = Visibility.Hidden;
        AddToCart.Visibility = Visibility.Hidden;
        UpdateCart.Visibility = Visibility.Hidden;
        UpdateAmountTB.Visibility = Visibility.Hidden;
        UpdateAmountL.Visibility = Visibility.Hidden;
    }


    public ProductView(BLApi.IBl? bl, int ViewProductID, BO.Cart cart)
    {
        InitializeComponent();
        _bl = bl;
        _cart = cart;
        Catgory.ItemsSource = Enum.GetValues(typeof(BO.Category));
        product = _bl?.Product.GetProductManger(ViewProductID)!;
        DataContext = product;
        Catgory.SelectedItem = product.Category;
        AddProduct.Visibility = Visibility.Hidden;
        UpdateProduct.Visibility = Visibility.Hidden;
        UpdateCart.Visibility = Visibility.Hidden;
        UpdateAmountTB.Visibility = Visibility.Hidden;
        UpdateAmountL.Visibility = Visibility.Hidden;
        Id.IsEnabled = false;
        Name.IsEnabled = false;
        Price.IsEnabled = false;
        Instock.IsEnabled = false;
        Catgory.IsEnabled = false;
    }

    public ProductView(BLApi.IBl? bl, int ViewProductID, BO.Cart cart, string str)
    {
        InitializeComponent();
        _bl = bl;
        _cart = cart;
        product = _bl?.Product.GetProductManger(ViewProductID)!;
        DataContext = product;
        Catgory.SelectedItem = product.Category;
        AddProduct.Visibility = Visibility.Hidden;
        UpdateProduct.Visibility = Visibility.Hidden;
        AddToCart.Visibility = Visibility.Hidden;
        Id.IsEnabled = false;
        Name.IsEnabled = false;
        Price.IsEnabled = false;
        Instock.IsEnabled = false;
    }

    /// <summary>
    /// Adding a product to the product list
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AddProductClick(object sender, RoutedEventArgs e)
    {
        BO.Product? product = ValidProduct();
        if (product == null)
            return;
        try
        {
            _bl?.Product.AddProduct(product);
            MessageBox.Show("SUCCSES", "SUCCSES", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
        catch (AddException ex)
        {
            MessageBox.Show(ex.Message + ex.InnerException!.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (NoValidException ex)
        {
            MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Product update in the product list
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void UpdateProductClick(object sender, RoutedEventArgs e)
    {
        product = ValidProduct();

        if (product == null)
            return;

        try
        {
            _bl?.Product.UpdateProduct(product);
            MessageBox.Show("SUCCSES", "SUCCSES", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();
        }
        catch (NoFoundException ex)
        {
            MessageBox.Show(ex.Message + ex.InnerException!.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (NoValidException ex)
        {
            MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Auxiliary function for basic input correctness check when adding or updating a product
    /// </summary>
    /// <returns> If everything is fine you will return a product entity to add or update </returns>
    private BO.Product? ValidProduct()
    {


        if (!int.TryParse(Instock.Text, out int n1) ||
            !int.TryParse(Id.Text, out int n2) ||
            !double.TryParse(Price.Text, out double n3) ||
            string.IsNullOrWhiteSpace(Name.Text))
        {
            MessageBox.Show("ERROR - ONE FIELD IN INCORECT INPUT", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }

        product = new BO.Product()
        {
            ProductID = int.Parse(Id.Text),
            ProductName = Name.Text,
            Category = (BO.Category)Catgory.SelectedItem,
            ProductPrice = double.Parse(Price.Text),
            InStock = int.Parse(Instock.Text)
        };
        return product;
    }

    private void AddToCart_Click(object sender, RoutedEventArgs e)
    {

        try
        {
            _cart = _bl?.Cart.AddProductToCart(_cart, (int)product?.ProductID!);
            this.Close();
            new PL.Cart.Cart(_cart).Show();
        } 
        catch (Exception)
        {
            MessageBox.Show("ERROR - ONE FIELD IN INCORECT INPUT", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateCart_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _cart = _bl?.Cart.UpdateAmount(_cart, (int)product?.ProductID!, int.Parse(UpdateAmountTB.Text));
            this.Close();
            new PL.Cart.Cart(_cart).Show();
        } 
        catch (Exception)
        {

            MessageBox.Show("ERROR - ONE FIELD IN INCORECT INPUT", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}