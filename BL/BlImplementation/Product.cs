﻿namespace BlImplementation;

/// <summary>
/// Implementation of product functions
/// </summary>
internal class Product : BLApi.IProduct
{
    /// <summary>
    /// Access to Dal
    /// </summary>
    private DalApi.IDal _dal = DalApi.Factory.Get()!;

    /// <summary>
    /// Returns a list of products - for manager
    /// </summary>
    /// <returns> IEnumerable<BO.ProductForList> </returns>
    public IEnumerable<BO.ProductForList?> GetProductList(Func<BO.ProductForList?, bool>? func = null)
    {
        lock (_dal)
        {
            var products = _dal?.Product.GetAll();
            bool flag = func is null;
            return (from item in products
                    select new BO.ProductForList()
                    {
                        ProductID = (int)item?.ProductID!,
                        ProductName = item?.Name,
                        Category = (BO.Category)item?.Category!,
                        ProductPrice = (double)item?.Price!
                    }).Where(element => flag ? flag : func!(element));
        }
    }

    /// <summary>
    ///  Returns a list of products - for  customer
    /// </summary>
    /// <param name="func"></param>
    /// <param name="cart"></param>
    /// <returns></returns>
    public IEnumerable<BO.ProductItem?> GetProductListCostumer(BO.Cart cart, Func<BO.ProductItem?, bool>? func = null)
    {
        lock (_dal)
        {
            var products = _dal?.Product.GetAll();
            bool flag = func is null;
            return (from item in products
                    select GetProductCostumer((int)item?.ProductID!, cart)).Where(element => flag ? flag : func!(element));
        }
    }

    /// <summary>
    /// Returns a product - for a manager
    /// </summary>
    /// <param name="productID"></param>
    /// <returns></returns>
    /// <exception cref="BO.NoValidException"></exception>
    /// <exception cref="BO.NoFoundException"></exception>
    public BO.Product GetProductManger(int productID)
    {
        lock (_dal)
        {
            if (productID <= 0)
            {
                throw new BO.NoValidException("product id");
            }

            try
            {
                DO.Product temp = (DO.Product)_dal?.Product.Get(productID)!;

                return new BO.Product()
                {
                    ProductPrice = temp.Price,
                    Category = (BO.Category)temp.Category!,
                    ProductName = temp.Name,
                    ProductID = productID,
                    InStock = temp.InStock
                };
            }
            catch (DO.NoFoundException ex)
            {
                throw new BO.NoFoundException(ex);
            }
        }
    }

    /// <summary>
    /// Returns a product - for a costumer
    /// </summary>
    /// <param name="productID"></param>
    /// <param name="cart"></param>
    /// <returns></returns>
    /// <exception cref="BO.NoValidException"></exception>
    /// <exception cref="BO.NoFoundException"></exception>
    public BO.ProductItem GetProductCostumer(int productID, BO.Cart cart)
    {
        lock (_dal)
        {
            if (productID <= 0)
            {
                throw new BO.NoValidException("product id");
            }

            try
            {
                DO.Product temp = (DO.Product)_dal?.Product.Get(productID)!;

                int amount = 0;
                if (cart.ItemsList is not null)
                {
                    foreach (var item in cart.ItemsList)
                    {
                        if (item!.ProductID == productID)
                        {
                            amount = item.Amount;
                        }
                    }
                }
                return new BO.ProductItem()
                {
                    ProductID = temp.ProductID,
                    ProductName = temp.Name,
                    ProductPrice = temp.Price,
                    Category = (BO.Category)temp.Category!,
                    InStock = (temp.InStock > 0) ? true : false,
                    AmountInCart = amount
                };
            }
            catch (DO.NoFoundException ex)
            {
                throw new BO.NoFoundException(ex);
            }
        }
    }

    /// <summary>
    /// add product
    /// </summary>
    /// <param name="productToAdd"></param>
    /// <exception cref="BO.NoValidException"></exception>
    /// <exception cref="BO.AddException"></exception>
    public void AddProduct(BO.Product productToAdd)
    {
        lock (_dal)
        {
            if (productToAdd.ProductID <= 0 ||
                productToAdd.ProductID.ToString().Length < 6 ||
                string.IsNullOrWhiteSpace(productToAdd.ProductID.ToString()))
            {
                throw new BO.NoValidException("product id");
            }

            if (string.IsNullOrWhiteSpace(productToAdd.ProductName) ||
                productToAdd.ProductPrice <= 0 ||
                productToAdd.InStock < 0)
            {
                throw new BO.NoValidException("name / price / stock");
            }

            DO.Product product = new()
            {
                ProductID = productToAdd.ProductID,
                Name = productToAdd.ProductName,
                Price = productToAdd.ProductPrice,
                InStock = productToAdd.InStock,
                Category = (DO.Category)productToAdd.Category!
            };

            try
            {
                _dal?.Product.Add(product);
            }
            catch (DO.AddException ex)
            {
                throw new BO.AddException(ex);
            }
        }
    }

    /// <summary>
    /// remove product
    /// </summary>
    /// <param name="productID"></param>
    /// <exception cref="BO.ErrorDeleteException"></exception>
    /// <exception cref="BO.NoFoundException"></exception>
    public void RemoveProduct(int productID)
    {
        lock (_dal)
        {
            if (_dal?.OrderItem.GetAll(item => item?.ProductID == productID) != null)
            {
                throw new BO.ErrorDeleteException("product in the order");
            }
            try
            {
                _dal?.Product.Delete(productID);
            }
            catch (DO.NoFoundException ex)
            {
                throw new BO.NoFoundException(ex);
            }
        }
    }

    /// <summary>
    /// update product
    /// </summary>
    /// <param name="productTOUpdate"></param>
    /// <exception cref="BO.NoValidException"></exception>
    /// <exception cref="BO.NoFoundException"></exception>
    public void UpdateProduct(BO.Product productTOUpdate)
    {
        lock (_dal)
        {
            if (productTOUpdate.ProductID <= 0 ||
                productTOUpdate.ProductID.ToString().Length < 6 ||
                string.IsNullOrWhiteSpace(productTOUpdate.ProductID.ToString()))
            {
                throw new BO.NoValidException("product id");
            }

            if (string.IsNullOrWhiteSpace(productTOUpdate.ProductName) ||
                productTOUpdate.ProductPrice <= 0 ||
                productTOUpdate.InStock < 0)
            {
                throw new BO.NoValidException("name / price / stock");
            }

            DO.Product product = new()
            {
                ProductID = productTOUpdate.ProductID,
                Name = productTOUpdate.ProductName,
                Category = (DO.Category)productTOUpdate.Category!,
                Price = productTOUpdate.ProductPrice,
                InStock = productTOUpdate.InStock
            };
            try
            {
                _dal?.Product.Update(product);
            }
            catch (DO.NoFoundException ex)
            {
                throw new BO.NoFoundException(ex);
            }
        }
    }
}