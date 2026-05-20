from ConexionBD.Conexion import Conexion, TipoMotor
from Modelo.Movimiento import Movimiento, Tipo
from datetime import datetime

class MovimientoBD:
    def insertar(self, m):
        sql = "INSERT INTO movimientos (tipo, descripcion, monto, fecha, usuario) VALUES (%s,%s,%s,%s,%s)"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                fecha_str = m.getFecha().strftime('%Y-%m-%d %H:%M:%S') if m.getFecha() else datetime.now().strftime('%Y-%m-%d %H:%M:%S')
                cursor.execute(sql, (m.getTipo().name, m.getDescripcion(), m.getMonto(), fecha_str, m.getUsuario()))
                conn.commit()
                res = cursor.rowcount > 0
                cursor.close()
                return res
        except Exception as e:
            print(e)
        return False

    def obtenerDelDia(self):
        lista = []
        if Conexion.getMotor() == TipoMotor.MYSQL:
            sql = "SELECT * FROM movimientos WHERE DATE(fecha) = CURDATE() ORDER BY fecha DESC"
        else:
            sql = "SELECT * FROM movimientos WHERE CAST(fecha AS DATE) = CAST(GETDATE() AS DATE) ORDER BY fecha DESC"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute(sql)
                rows = cursor.fetchall()
                cursor.close()
                for row in rows:
                    lista.append(self.mapear(row))
        except Exception as e:
            print(e)
        return lista

    def obtenerTodos(self):
        lista = []
        sql = "SELECT * FROM movimientos ORDER BY fecha DESC"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute(sql)
                rows = cursor.fetchall()
                cursor.close()
                for row in rows:
                    lista.append(self.mapear(row))
        except Exception as e:
            print(e)
        return lista

    def mapear(self, row):
        m = Movimiento()
        m.setTipo(Tipo[row["tipo"]])
        m.setDescripcion(row["descripcion"])
        m.setMonto(float(row["monto"]))
        m.setFecha(row["fecha"])
        m.setUsuario(row["usuario"])
        return m
