from ConexionBD.Conexion import Conexion
from Modelo.OrdenCompra import OrdenCompra, TipoPago, Estado
from Modelo.Producto import Producto
from Modelo.Proveedor import Proveedor
from datetime import datetime

class OrdenCompraBD:
    def insertar(self, orden):
        conn = Conexion.getConexion()
        if not conn: return False
        sqlOrden = "INSERT INTO ordenes_compra (proveedor_id, total, tipo_pago, estado, fecha) VALUES (%s,%s,%s,%s,%s)"
        sqlItem = "INSERT INTO orden_items (orden_id, producto_id, cantidad_solicitada, precio_costo) VALUES (%s,%s,%s,%s)"
        try:
            conn.autocommit = False
            cursor = conn.cursor()
            fecha_str = orden.getFecha().strftime('%Y-%m-%d %H:%M:%S') if orden.getFecha() else datetime.now().strftime('%Y-%m-%d %H:%M:%S')
            cursor.execute(sqlOrden, (orden.getProveedor().getId(), orden.getTotal(), orden.getTipoPago().name, orden.getEstado().name, fecha_str))
            
            idOrden = cursor.lastrowid
            if not idOrden:
                raise Exception("No se genero ID para la orden")
            orden.setId(idOrden)
            
            datos_items = []
            for item in orden.getItems():
                datos_items.append((idOrden, item.getProducto().getId(), item.getCantidadSolicitada(), item.getPrecioCosto()))
            
            cursor.executemany(sqlItem, datos_items)
            conn.commit()
            cursor.close()
            return True
        except Exception as e:
            try:
                conn.rollback()
            except Exception:
                pass
            print(e)
            return False
        finally:
            try:
                conn.autocommit = True
            except Exception:
                pass

    def marcarRecibida(self, id_orden):
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                cursor.execute("UPDATE ordenes_compra SET estado = 'RECIBIDO' WHERE id = %s", (id_orden,))
                conn.commit()
                res = cursor.rowcount > 0
                cursor.close()
                return res
        except Exception as e:
            print(e)
        return False

    def obtenerTodas(self):
        lista = []
        sql = "SELECT oc.*, p.nombre AS prov_nombre, p.contacto, p.telefono, p.email, p.direccion FROM ordenes_compra oc JOIN proveedores p ON oc.proveedor_id = p.id ORDER BY oc.fecha DESC"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute(sql)
                rows = cursor.fetchall()
                cursor.close()
                for rs in rows:
                    prov = Proveedor(rs["proveedor_id"], rs["prov_nombre"], rs["contacto"], rs["telefono"], rs["email"], rs["direccion"], True)
                    oc = OrdenCompra(rs["id"], prov, self.obtenerItems(rs["id"]), float(rs["total"]), TipoPago[rs["tipo_pago"]], Estado[rs["estado"]], rs["fecha"])
                    lista.append(oc)
        except Exception as e:
            print(e)
        return lista

    def buscarPorId(self, id_orden):
        sql = "SELECT oc.*, p.nombre AS prov_nombre, p.contacto, p.telefono, p.email, p.direccion FROM ordenes_compra oc JOIN proveedores p ON oc.proveedor_id = p.id WHERE oc.id = %s"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute(sql, (id_orden,))
                rs = cursor.fetchone()
                cursor.close()
                if rs:
                    prov = Proveedor(rs["proveedor_id"], rs["prov_nombre"], rs["contacto"], rs["telefono"], rs["email"], rs["direccion"], True)
                    return OrdenCompra(rs["id"], prov, self.obtenerItems(id_orden), float(rs["total"]), TipoPago[rs["tipo_pago"]], Estado[rs["estado"]], rs["fecha"])
        except Exception as e:
            print(e)
        return None

    def obtenerItems(self, ordenId):
        items = []
        sql = "SELECT oi.*, p.nombre, p.precio, p.categoria, p.unidad FROM orden_items oi JOIN productos p ON oi.producto_id = p.id WHERE oi.orden_id = %s"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute(sql, (ordenId,))
                rows = cursor.fetchall()
                cursor.close()
                for rs in rows:
                    prod = Producto()
                    prod.setId(rs["producto_id"])
                    prod.setNombre(rs["nombre"])
                    prod.setPrecio(float(rs["precio"]))
                    prod.setStock(0)
                    prod.setCategoria(rs["categoria"])
                    prod.setUnidad(rs["unidad"])
                    prod.setImagenRuta("")
                    item = OrdenCompra.ItemOrden(prod, float(rs["cantidad_solicitada"]), float(rs["precio_costo"]))
                    items.append(item)
        except Exception as e:
            print(e)
        return items
