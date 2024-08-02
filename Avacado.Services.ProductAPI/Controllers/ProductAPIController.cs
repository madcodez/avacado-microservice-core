using Avacado.Services.ProductAPI.Models.Dto;
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
        public object Post([FromForm] ProductDto productDto)
        {
            try
            {
                Product obj = _mapper.Map<Product>(productDto);
                _db.Products.Add(obj);
                _db.SaveChanges();

				if (productDto.Image != null)
				{

					string fileName = obj.Id + Path.GetExtension(productDto.Image.FileName);
					string filePath = @"wwwroot\ProductImages\" + fileName;

					//I have added the if condition to remove the any image with same name if that exist in the folder by any change
					var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);
					FileInfo file = new FileInfo(directoryLocation);
					if (file.Exists)
					{
						file.Delete();
					}

					var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
					using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
					{
						productDto.Image.CopyTo(fileStream);
					}
					var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
					obj.ImageUrl = baseUrl + "/ProductImages/" + fileName;
					obj.ImageLocalPath = filePath;
				}
				else
				{
					obj.ImageUrl = "https://placehold.co/600x400";
				}
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
        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public object Put([FromForm] ProductDto productDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDto);
                _db.Products.Update(product);
                _db.SaveChanges();

                if (productDto.Image != null)
                {
                    if (!string.IsNullOrEmpty(product.ImageLocalPath))
                    {
                        var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                        FileInfo file = new FileInfo(oldFilePathDirectory);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    string fileName = product.Id + Path.GetExtension(productDto.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;
                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        productDto.Image.CopyTo(fileStream);
                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }


                _db.Products.Update(product);
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

                if (!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePathDirectory);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

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

