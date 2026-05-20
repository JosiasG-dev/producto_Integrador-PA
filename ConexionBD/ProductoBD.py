from ConexionBD.Conexion import Conexion
from Modelo.Producto import Producto
import re

class ProductoBD:
    def obtenerTodos(self):
        lista = []
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute("SELECT * FROM productos ORDER BY CAST(id AS UNSIGNED)")
                rows = cursor.fetchall()
                cursor.close()
                for row in rows:
                    lista.append(self.mapear(row))
        except Exception as e:
            print(e)
        return lista

    def buscarPorId(self, id_str):
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute("SELECT * FROM productos WHERE id = %s", (id_str,))
                row = cursor.fetchone()
                cursor.close()
                if row:
                    return self.mapear(row)
        except Exception as e:
            print(e)
        return None

    def buscar(self, texto):
        lista = []
        if texto is None or not str(texto).strip():
            return lista
        txt = str(texto).strip()
        esSoloNumero = bool(re.match(r"^\d+$", txt))
        try:
            conn = Conexion.getConexion()
            if conn:
                if esSoloNumero:
                    skuFormateado = f"{int(txt):03d}"
                    cursor = conn.cursor(dictionary=True)
                    cursor.execute("SELECT * FROM productos WHERE id = %s OR id = %s LIMIT 10", (txt, skuFormateado))
                    rows = cursor.fetchall()
                    cursor.close()
                    for row in rows:
                        lista.append(self.mapear(row))
                    if not lista:
                        cursor = conn.cursor(dictionary=True)
                        cursor.execute("SELECT * FROM productos WHERE nombre LIKE %s LIMIT 10", (f"%{txt}%",))
                        rows = cursor.fetchall()
                        cursor.close()
                        for row in rows:
                            lista.append(self.mapear(row))
                else:
                    cursor = conn.cursor(dictionary=True)
                    cursor.execute("SELECT * FROM productos WHERE nombre LIKE %s LIMIT 10", (f"%{txt}%",))
                    rows = cursor.fetchall()
                    cursor.close()
                    for row in rows:
                        lista.append(self.mapear(row))
        except Exception as e:
            print(e)
        return lista

    def insertar(self, p):
        sql = "INSERT INTO productos (id, nombre, precio, stock, stock_minimo, categoria, unidad, imagen_ruta) VALUES (%s,%s,%s,%s,%s,%s,%s,%s)"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                cursor.execute(sql, (p.getId(), p.getNombre(), p.getPrecio(), p.getStock(), p.getStockMinimo(), p.getCategoria(), p.getUnidad(), p.getImagenRuta()))
                conn.commit()
                res = cursor.rowcount > 0
                cursor.close()
                return res
        except Exception as e:
            print(e)
        return False

    def actualizar(self, p):
        sql = "UPDATE productos SET nombre=%s, precio=%s, stock=%s, stock_minimo=%s, categoria=%s, unidad=%s, imagen_ruta=%s WHERE id=%s"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                cursor.execute(sql, (p.getNombre(), p.getPrecio(), p.getStock(), p.getStockMinimo(), p.getCategoria(), p.getUnidad(), p.getImagenRuta(), p.getId()))
                conn.commit()
                res = cursor.rowcount > 0
                cursor.close()
                return res
        except Exception as e:
            print(e)
        return False

    def actualizarStock(self, id_str, nuevoStock):
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                cursor.execute("UPDATE productos SET stock=%s WHERE id=%s", (nuevoStock, id_str))
                conn.commit()
                res = cursor.rowcount > 0
                cursor.close()
                return res
        except Exception as e:
            print(e)
        return False

    def eliminar(self, id_str):
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                cursor.execute("DELETE FROM productos WHERE id=%s", (id_str,))
                conn.commit()
                res = cursor.rowcount > 0
                cursor.close()
                return res
        except Exception as e:
            print(e)
        return False

    def generarNuevoId(self):
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute("SELECT MAX(CAST(id AS UNSIGNED)) AS max_id FROM productos")
                row = cursor.fetchone()
                cursor.close()
                if row and row["max_id"] is not None:
                    return f"{int(row['max_id']) + 1:03d}"
        except Exception as e:
            print(e)
        return "001"

    def mapear(self, row):
        precio = float(row["precio"]) if row["precio"] is not None else 0.0
        p = Producto()
        p.setId(row["id"])
        p.setNombre(row["nombre"])
        p.setPrecio(precio)
        p.setStock(float(row["stock"]))
        p.setCategoria(row["categoria"])
        p.setUnidad(row["unidad"])
        p.setImagenRuta(row["imagen_ruta"])
        p.setStockMinimo(float(row["stock_minimo"]))
        return p

    def actualizarImagenes(self, productos):
        sql = "UPDATE productos SET imagen_ruta=%s WHERE id=%s AND (imagen_ruta IS NULL OR imagen_ruta='')"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                datos = [(p.getImagenRuta(), p.getId()) for p in productos]
                cursor.executemany(sql, datos)
                conn.commit()
                cursor.close()
        except Exception as e:
            print(e)
