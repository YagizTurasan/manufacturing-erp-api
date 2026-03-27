using BahadirMakina.Application.DTOs.Stok;
using BahadirMakina.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BahadýrMakina.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StokController : ControllerBase
    {
        private readonly IStokService _stokService;

        public StokController(IStokService stokService)
        {
            _stokService = stokService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _stokService.GetAllAsync();
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("depo/{depoId}")]
        public async Task<IActionResult> GetByDepo(int depoId)
        {
            var result = await _stokService.GetByDepoIdAsync(depoId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("urun/{urunId}")]
        public async Task<IActionResult> GetByUrun(int urunId)
        {
            var result = await _stokService.GetByUrunIdAsync(urunId);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPost("giris")]
        public async Task<IActionResult> StokGirisi([FromBody] StokGirisiDto dto)
        {
            var result = await _stokService.StokGirisiAsync(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPost("cikis")]
        public async Task<IActionResult> StokCikisi([FromBody] StokCikisiDto dto)
        {
            var result = await _stokService.StokCikisiAsync(dto);
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpGet("dusuk-stok")]
        public async Task<IActionResult> GetDusukStoklar()
        {
            var result = await _stokService.GetDusukStoklarAsync();
            if (!result.Success)
                return BadRequest(result);
            return Ok(result);
        }
        [HttpPost("transfer")]
        public async Task<IActionResult> StokTransfer([FromBody] StokTransferDto dto)
        {
            var result = await _stokService.StokTransferAsync(dto);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("sayim")]
        public async Task<IActionResult> StokSayim([FromBody] StokSayimDto dto)
        {
            var result = await _stokService.StokSayimAsync(dto);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
