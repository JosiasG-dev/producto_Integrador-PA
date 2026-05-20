from ConexionBD.Conexion import Conexion
from Modelo.ConfiguracionTienda import ConfiguracionTienda

class ConfiguracionBD:
    def obtener(self):
        sql = "SELECT * FROM configuracion WHERE id = 1"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute(sql)
                row = cursor.fetchone()
                cursor.close()
                if row:
                    config = ConfiguracionTienda()
                    config.setNombre(row["nombre_tienda"])
                    config.setSucursal(row["sucursal"])
                    config.setRfc(row["rfc"])
                    return config
        except Exception as e:
            print(e)
        return None

    def actualizar(self, config):
        sql = "UPDATE configuracion SET nombre_tienda=%s, sucursal=%s, rfc=%s WHERE id=1"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                cursor.execute(sql, (config.getNombre(), config.getSucursal(), config.getRfc()))
                conn.commit()
                filasAfectadas = cursor.rowcount
                cursor.close()
                print(f"Filas actualizadas en BD: {filasAfectadas}")
        except Exception as e:
            print(e)
