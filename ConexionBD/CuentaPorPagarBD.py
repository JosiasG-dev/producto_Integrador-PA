from ConexionBD.Conexion import Conexion
from Modelo.CuentaPorPagar import CuentaPorPagar, Estado
from Modelo.Proveedor import Proveedor

class CuentaPorPagarBD:
    def insertar(self, c):
        sql = "INSERT INTO cuentas_por_pagar (proveedor_id, orden_id, monto_total, pagado, saldo, vencimiento, estado) VALUES (%s,%s,%s,%s,%s,%s,%s)"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                cursor.execute(sql, (c.getProveedor().getId(), c.getOrdenId(), c.getMontoTotal(), c.getPagado(), c.getSaldo(), c.getVencimiento(), c.getEstado().name))
                conn.commit()
                if cursor.rowcount > 0:
                    c.setId(cursor.lastrowid)
                    cursor.close()
                    return True
                cursor.close()
        except Exception as e:
            print(e)
        return False

    def aplicarPago(self, id_cpp, montoPagado, nuevoSaldo, nuevoEstado):
        sql = "UPDATE cuentas_por_pagar SET pagado=%s, saldo=%s, estado=%s WHERE id=%s"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                cursor.execute(sql, (montoPagado, nuevoSaldo, nuevoEstado, id_cpp))
                conn.commit()
                res = cursor.rowcount > 0
                cursor.close()
                return res
        except Exception as e:
            print(e)
        return False

    def obtenerActivas(self):
        lista = []
        sql = "SELECT cpp.*, p.nombre AS prov_nombre, p.contacto, p.telefono, p.email, p.direccion FROM cuentas_por_pagar cpp JOIN proveedores p ON cpp.proveedor_id = p.id WHERE cpp.estado != 'PAGADO' ORDER BY cpp.id DESC"
        return self.ejecutarQuery(lista, sql)

    def obtenerTodas(self):
        lista = []
        sql = "SELECT cpp.*, p.nombre AS prov_nombre, p.contacto, p.telefono, p.email, p.direccion FROM cuentas_por_pagar cpp JOIN proveedores p ON cpp.proveedor_id = p.id ORDER BY cpp.id DESC"
        return self.ejecutarQuery(lista, sql)

    def ejecutarQuery(self, lista, sql):
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute(sql)
                rows = cursor.fetchall()
                cursor.close()
                for rs in rows:
                    prov = Proveedor(rs["proveedor_id"], rs["prov_nombre"], rs["contacto"], rs["telefono"], rs["email"], rs["direccion"], True)
                    c = CuentaPorPagar(rs["id"], prov, rs["orden_id"], float(rs["monto_total"]), float(rs["pagado"]), float(rs["saldo"]), rs["vencimiento"], Estado[rs["estado"]])
                    lista.append(c)
        except Exception as e:
            print(e)
        return lista
