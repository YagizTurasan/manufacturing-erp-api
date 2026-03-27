using BahadirMakina.Application.DTOs.IsAdimi;

namespace BahadirMakina.Application.DTOs.IsEmri;

public class IsEmriDetayDto : IsEmriDto
{
    public List<IsAdimiDto> IsAdimlari { get; set; } = new();
}