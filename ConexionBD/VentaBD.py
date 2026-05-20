from ConexionBD.Conexion import Conexion
from Modelo.ItemCarrito import ItemCarrito
from Modelo.Producto import Producto
from Modelo.Venta import Venta
from ConexionBD.ProductoBD import ProductoBD
from datetime import datetime

class VentaBD:
    def insertar(self, venta):
        conn = Conexion.getConexion()
        if not conn: return False
        sqlVenta = "INSERT INTO ventas (total, descuento, metodo_pago, fecha, cajero) VALUES (%s,%s,%s,%s,%s)"
        sqlItem = "INSERT INTO venta_items (venta_id, producto_id, cantidad, precio_unit) VALUES (%s,%s,%s,%s)"
        try:
            conn.autocommit = False
            cursor = conn.cursor()
            
            fecha_str = venta.getFecha().strftime('%Y-%m-%d %H:%M:%S') if venta.getFecha() else datetime.now().strftime('%Y-%m-%d %H:%M:%S')
            cursor.execute(sqlVenta, (venta.getTotal(), venta.getDescuento(), venta.getMetodoPago(), fecha_str, venta.getCajero()))
            
            idVenta = cursor.lastrowid
            if not idVenta:
                raise Exception("No se genero ID para la venta")
            venta.setId(idVenta)
            
            datos_items = []
            for item in venta.getItems():
                datos_items.append((idVenta, item.getProducto().getId(), item.getCantidad(), item.getProducto().getPrecio()))
            
            cursor.executemany(sqlItem, datos_items)
            cursor.close()
            
            productoBD = ProductoBD()
            for item in venta.getItems():
                p = item.getProducto()
                nuevoStock = round(p.getStock() - item.getCantidad(), 3)
                productoBD.actualizarStock(p.getId(), nuevoStock)
                p.setStock(nuevoStock)
                
            conn.commit()
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

    def obtenerTodas(self):
        lista = []
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute("SELECT * FROM ventas ORDER BY fecha DESC")
                rows = cursor.fetchall()
                cursor.close()
                for row in rows:
                    v = Venta()
                    v.setId(row["id"])
                    v.setTotal(float(row["total"]))
                    v.setDescuento(float(row["descuento"]))
                    v.setMetodoPago(row["metodo_pago"])
                    v.setFecha(row["fecha"])
                    v.setCajero(row["cajero"])
                    v.setItems(self.obtenerItems(row["id"]))
                    lista.append(v)
        except Exception as e:
            print(e)
        return lista

    def obtenerItems(self, ventaId):
        items = []
        sql = "SELECT vi.*, p.nombre, p.precio, p.categoria, p.unidad FROM venta_items vi JOIN productos p ON vi.producto_id = p.id WHERE vi.venta_id = %s"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute(sql, (ventaId,))
                rows = cursor.fetchall()
                cursor.close()
                for row in rows:
                    p = Producto()
                    p.setId(row["producto_id"])
                    p.setNombre(row["nombre"])
                    p.setPrecio(float(row["precio_unit"]))
                    p.setStock(0)
                    p.setCategoria(row["categoria"])
                    p.setUnidad(row["unidad"])
                    p.setImagenRuta("")
                    item = ItemCarrito()
                    item.setProducto(p)
                    item.setCantidad(float(row["cantidad"]))
                    items.append(item)
        except Exception as e:
            print(e)
        return items
