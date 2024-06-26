﻿using BankAPI.Models;
using BankAPI.Services;
using Microsoft.AspNetCore.Mvc;
namespace BankAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("{accountId}/transactions")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions(int accountId)
        {
            var transactions = await _accountService.GetTransactionsAsync(accountId);
            return Ok(transactions);
        }

        [HttpPost("{accountId}/withdraw")]
        public async Task<IActionResult> Withdraw(int accountId, [FromBody] WithdrawRequest request, [FromQuery] bool isExternalBank)
        {
            try
            {
                await _accountService.WithdrawAsync(accountId, request.Amount, isExternalBank);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{accountId}/deposit")]
        public async Task<IActionResult> Deposit(int accountId, [FromBody] DepositRequest request)
        {
            try
            {
                await _accountService.DepositAsync(accountId, request.Amount);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{accountId}/transfer")]
        public async Task<IActionResult> Transfer(int accountId, [FromBody] TransferRequest request)
        {
            try
            {
                await _accountService.TransferAsync(accountId, request.DestinationIBAN, request.Amount);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{accountId}/activateCard")]
        public async Task<IActionResult> Activate(int accountId)
        {
            try
            {
                await _accountService.ActivateAsync(accountId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{accountId}/changePIN")]
        public async Task<IActionResult> ChangePIN(int accountId, [FromBody] ChangePINRequest request)
        {
            try
            {
                await _accountService.ChangePINAsync(accountId, request.NewPIN);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}