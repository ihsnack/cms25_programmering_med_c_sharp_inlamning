﻿using Infrastructure.Interfaces;

namespace Infrastructure.Models;

public class ResponseResult<T> : IResponseResult<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Result { get; set; }
}
