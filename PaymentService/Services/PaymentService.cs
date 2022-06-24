using System;
using System.Threading.Tasks;
using AutoMapper;
using Common.Extensions.Exceptions;
using Common.Models.Dtos;
using Common.Services.Repositories;
using PaymentService.Models.Entities;
using PaymentService.Repositories;

namespace PaymentService.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _repository;

    private readonly IMapper _mapper;

    public PaymentService(
        IPaymentRepository repository, 
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaymentDto> GetByIdAsync(Guid id)
    {
        var payment = await _repository.GetByIdAsync<Guid, Payment>(id);
        if (payment == null)
            throw new NotFoundException();

        return _mapper.Map<PaymentDto>(payment);
    }

    public async Task<PaymentDto> GetByRentalIdAsync(Guid rentalId)
    {
        var payment = await _repository.GetByRentalIdAsync(rentalId);
        if (payment == null)
            throw new NotFoundException();

        return _mapper.Map<PaymentDto>(payment);
    }

    public async Task<PaymentDto> CreateAsync(PaymentDto paymentDto)
    {
        var entity = _mapper.Map<Payment>(paymentDto);

        var result = await _repository.CreateAsync<Guid, Payment>(entity);

        return _mapper.Map<PaymentDto>(result);
    }

    public async Task<PaymentDto> UpdateAsync(Guid id, PaymentDto paymentDto)
    {
        var payment = await _repository.GetByIdAsync<Guid, Payment>(id);
        if (payment == null)
            throw new NotFoundException();

        payment = _mapper.Map<Payment>(paymentDto);

        var result = await _repository.UpdateAsync(id, payment);

        return _mapper.Map<PaymentDto>(result);
    }
}