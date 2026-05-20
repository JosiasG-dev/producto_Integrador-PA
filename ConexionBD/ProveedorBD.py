from ConexionBD.Conexion import Conexion
from Modelo.Proveedor import Proveedor

class ProveedorBD:
    def obtenerTodos(self):
        lista = []
        sql = "SELECT * FROM proveedores WHERE activo = 1 ORDER BY id"
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

    def buscarPorId(self, id_prov):
        sql = "SELECT * FROM proveedores WHERE id = %s"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute(sql, (id_prov,))
                row = cursor.fetchone()
                cursor.close()
                if row:
                    return self.mapear(row)
        except Exception as e:
            print(e)
        return None

    def insertar(self, p):
        sql = "INSERT INTO proveedores (nombre, contacto, telefono, email, direccion, activo) VALUES (%s,%s,%s,%s,%s,%s)"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                cursor.execute(sql, (p.getNombre(), p.getContacto(), p.getTelefono(), p.getEmail(), p.getDireccion(), p.isActivo()))
                conn.commit()
                if cursor.rowcount > 0:
                    p.setId(cursor.lastrowid)
                    cursor.close()
                    return True
                cursor.close()
        except Exception as e:
            print(e)
        return False

    def actualizar(self, p):
        sql = "UPDATE proveedores SET nombre=%s, contacto=%s, telefono=%s, email=%s, direccion=%s WHERE id=%s"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                cursor.execute(sql, (p.getNombre(), p.getContacto(), p.getTelefono(), p.getEmail(), p.getDireccion(), p.getId()))
                conn.commit()
                res = cursor.rowcount > 0
                cursor.close()
                return res
        except Exception as e:
            print(e)
        return False

    def eliminar(self, id_prov):
        sql = "UPDATE proveedores SET activo = 0 WHERE id = %s"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                cursor.execute(sql, (id_prov,))
                conn.commit()
                res = cursor.rowcount > 0
                cursor.close()
                return res
        except Exception as e:
            print(e)
        return False

    def mapear(self, row):
        p = Proveedor()
        p.setId(row["id"])
        p.setNombre(row["nombre"])
        p.setContacto(row["contacto"])
        p.setTelefono(row["telefono"])
        p.setEmail(row["email"])
        p.setDireccion(row["direccion"])
        p.setActivo(bool(row["activo"]))
        return p
