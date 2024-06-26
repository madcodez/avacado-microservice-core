﻿using Avacado.Services.ProductAPI.Models.Dto;
using Avacado.Services.ProductAPI.Data;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Avacado.Services.ProductAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace Avacado.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    [Authorize]
      public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ResponseDto _response;
        private IMapper _mapper;

        public ProductAPIController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
        }
        [HttpGet]
        public object Get()
        {
            try
            {
                IEnumerable<Product> objList = _db.Products.ToList();
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(objList);


            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }
        [HttpGet]
        [Route("{id:int}")]
        public object Get(int id)
        {
            try
            {
                Product Product = _db.Products.First(i => i.Id == id);
                _response.Result = _mapper.Map<ProductDto>(Product);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public object Post([FromBody] ProductDto productDto)
        {
            try
            {
                Product obj = _mapper.Map<Product>(productDto);
                _db.Products.Add(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(obj);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }
        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public object Put([FromBody] ProductDto productDto)
        {
            try
            {
                Product obj = _mapper.Map<Product>(productDto);
                _db.Products.Update(obj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(obj);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public object Delete(int id)
        {
            try
            {
                Product product = _db.Products.First(i => i.Id == id);
                _db.Products.Remove(product);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(product);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }

    }

}

