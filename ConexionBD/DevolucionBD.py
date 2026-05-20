from ConexionBD.Conexion import Conexion
from Modelo.Devolucion import Devolucion
from Modelo.Producto import Producto
from datetime import datetime

class DevolucionBD:
    def insertar(self, d):
        sql = "INSERT INTO devoluciones (venta_id, producto_id, cantidad, motivo, fecha, cajero) VALUES (%s,%s,%s,%s,%s,%s)"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                fecha_str = d.getFecha().strftime('%Y-%m-%d %H:%M:%S') if d.getFecha() else datetime.now().strftime('%Y-%m-%d %H:%M:%S')
                cursor.execute(sql, (d.getVentaId(), d.getProducto().getId(), d.getCantidad(), d.getMotivo(), fecha_str, d.getCajero()))
                conn.commit()
                if cursor.rowcount > 0:
                    d.setId(cursor.lastrowid)
                    cursor.close()
                    return True
                cursor.close()
        except Exception as e:
            print(e)
        return False

    def obtenerPorVenta(self, ventaId):
        lista = []
        sql = "SELECT d.*, p.nombre, p.precio, p.categoria, p.unidad, p.imagen_ruta FROM devoluciones d JOIN productos p ON d.producto_id = p.id WHERE d.venta_id = %s"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute(sql, (ventaId,))
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
                    prod.setImagenRuta(rs["imagen_ruta"])
                    
                    dev = Devolucion()
                    dev.setId(rs["id"])
                    dev.setVentaId(ventaId)
                    dev.setProducto(prod)
                    dev.setCantidad(float(rs["cantidad"]))
                    dev.setMotivo(rs["motivo"])
                    dev.setFecha(rs["fecha"])
                    dev.setCajero(rs["cajero"])
                    lista.append(dev)
        except Exception as e:
            print(e)
        return lista

    def obtenerTodas(self):
        lista = []
        sql = "SELECT d.*, p.nombre, p.precio, p.categoria, p.unidad, p.imagen_ruta FROM devoluciones d JOIN productos p ON d.producto_id = p.id ORDER BY d.fecha DESC"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute(sql)
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
                    prod.setImagenRuta(rs["imagen_ruta"])
                    
                    dev = Devolucion()
                    dev.setId(rs["id"])
                    dev.setVentaId(rs["venta_id"])
                    dev.setProducto(prod)
                    dev.setCantidad(float(rs["cantidad"]))
                    dev.setMotivo(rs["motivo"])
                    dev.setFecha(rs["fecha"])
                    dev.setCajero(rs["cajero"])
                    lista.append(dev)
        except Exception as e:
            print(e)
        return lista
