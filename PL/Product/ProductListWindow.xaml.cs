﻿namespace PL.Product;

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

/// <summary>
/// Interaction logic for Products.xaml
/// </summary>
public partial class ProductList : Window
{
    /// <summary>
    /// Access to the logical layer
    /// </summary>
    private BLApi.IBl? _bl = BLApi.Factory.Get();

    /// <summary>
    ///Dependency Property for Category
    /// </summary>
    public static readonly DependencyProperty CategoryProp = DependencyProperty.Register(nameof(Category), typeof(BO.Category?), typeof(ProductList));

    /// <summary>
    /// object for a category
    /// </summary>
    public BO.Category? Category { get => (BO.Category?)GetValue(CategoryProp); set => SetValue(CategoryProp, value); }

    /// <summary>
    /// Displaying the list of categories via a display link
    /// </summary>
    public static BO.Category[] Categories { get; } = (BO.Category[])Enum.GetValues(typeof(BO.Category));

    /// <summary>
    /// Saving the list of products
    /// </summary>
    public static readonly DependencyProperty ListPropProduct = DependencyProperty.Register(nameof(Products), typeof(IEnumerable<BO.ProductForList?>), typeof(ProductList));

    /// <summary>
    /// For the list of products
    /// </summary>
    public IEnumerable<BO.ProductForList?> Products { get => (IEnumerable<BO.ProductForList?>)GetValue(ListPropProduct); set => SetValue(ListPropProduct, value); }

    /// <summary>
    /// constructor for product list
    /// </summary>
    public ProductList()
    {
        InitializeComponent();
        Category = null;
        Products = _bl!.Product.GetProductList()!;
    }

    /// <summary>
    /// Getting the updated list from the logical layer
    /// </summary>
    private void updateProductList()
    {
        if (Category == null)
            Products = _bl!.Product.GetProductList();
        else
            Products = _bl!.Product.GetProductList(item => item!.Category == Category);
    }

    /// <summary>
    /// Filter the list of products by category
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FilterProductsClick(object sender, SelectionChangedEventArgs e) => updateProductList();

    /// <summary>
    /// Access for adding a product
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AccessAddProduct(object sender, RoutedEventArgs e) => new ProductView(updateProductList).Show();

    /// <summary>
    /// Refreshing the list of products and presenting without filtering
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AllCatgoryClick(object sender, RoutedEventArgs e)
    {
        Category = null;
        updateProductList();
    }

    /// <summary>
    /// Access for product update
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void AccessUpdateProduct(object sender, MouseButtonEventArgs e)
    {
        BO.ProductForList productForList = (BO.ProductForList)ProductListview.SelectedItem;
        if (IsMouseCaptureWithin && productForList is not null)
            new ProductView(updateProductList, (productForList).ProductID).Show();
    }

    /// <summary>
    /// Sort the list by column
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SortByColmun(object sender, RoutedEventArgs e)
    {
        GridViewColumnHeader gridViewColumnHeader = (sender as GridViewColumnHeader)!;
        if (gridViewColumnHeader is not null)
        {
            string tag = (gridViewColumnHeader.Tag as string)!;
            ProductListview.Items.SortDescriptions.Clear();
            ProductListview.Items.SortDescriptions.Add(new SortDescription(tag, ListSortDirection.Ascending));
        }
    }

    /// <summary>
    /// Apply clicking only is the mouse on the selected item
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ProductListview_MouseMove(object sender, MouseEventArgs e)
    {
        ProductListview.SelectedItem = null;
    }
}