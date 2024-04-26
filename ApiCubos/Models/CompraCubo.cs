using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCubos.Models
{
    [Table("COMPRACUBOS")]
    public class CompraCubo
    {
        [Key]
        [Column("id_pedido")]
        public int IdPedido { get; set; }
        [Column("id_cubo")]
        public int IdCubo { get; set; }
        [Column("id_usuario")]
        public int Idusuario { get; set; }
        [Column("fechapedido")]
        public DateTime Fecha { get; set; }
    }
}
