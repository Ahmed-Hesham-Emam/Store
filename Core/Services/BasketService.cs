using AutoMapper;
using Domain.Contracts;
using Domain.Exceptions;
using Domain.Models;
using Services.Abstractions;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
    {
    public class BasketService(IBasketRepository basketRepository, IMapper mapper, IUnitOfWork unitOfWork) : IBasketService
        {

        #region Get
        public async Task<BasketDto?> GetCustomerBasketAsync(string id)
            {
            var basket = await basketRepository.GetBasketAsync(id);
            if ( basket is null ) throw new BasketNotFoundException(id);

            var result = mapper.Map<BasketDto>(basket);
            return result;
            }
        #endregion

        #region Update
        public async Task<BasketDto?> UpdateBasketAsync(BasketDto basket)
            {


            var testBasket = await basketRepository.GetBasketAsync(basket.Id); // Domain/Cart
            var existingBasket = mapper.Map<BasketDto>(testBasket); //CartDto


            if ( existingBasket is null ) existingBasket = new BasketDto { Id = basket.Id, Items = new List<BasketItemDto>() };
            foreach ( var item in basket.Items )
                {
                var product = await unitOfWork.GetRepository<Product, int>().GetAsync(item.Id);
                var existingItem = existingBasket.Items.FirstOrDefault(i => i.Id == item.Id);

                if ( existingItem is not null )
                    {
                    existingItem.Price = product.Price;
                    existingItem.Quantity += item.Quantity;
                    }
                else
                    {
                    item.Price = product.Price;
                    item.ProductName = product.Name;
                    item.PictureUrl = product.PictureUrl;

                    existingBasket.Items.Add(item);
                    }
                }
            basket = existingBasket;


            await basketRepository.UpdateBasketAsync(mapper.Map<CustomerBasket>(basket));

            return await GetCustomerBasketAsync(basket.Id);

            }
        #endregion

        #region Delete
        public async Task<bool> DeleteBasketAsync(string id)
            {
            var flag = await basketRepository.DeleteBasketAsync(id);
            if ( !flag ) throw new BasketDeleteBadRequest();

            return flag;
            }
        #endregion

        }
    }
