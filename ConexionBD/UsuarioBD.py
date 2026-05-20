from ConexionBD.Conexion import Conexion
from Modelo.Usuario import Usuario

class UsuarioBD:
    def obtenerTodos(self):
        lista = []
        sql = "SELECT * FROM usuarios ORDER BY id"
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

    def autenticar(self, username, password):
        sql = "SELECT * FROM usuarios WHERE username = %s AND password = %s"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor(dictionary=True)
                cursor.execute(sql, (username, password))
                row = cursor.fetchone()
                cursor.close()
                if row:
                    return self.mapear(row)
        except Exception as e:
            print(e)
        return None

    def insertar(self, u):
        sql = "INSERT INTO usuarios (username, password, rol, nombre, edad, sexo) VALUES (%s,%s,%s,%s,%s,%s)"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                cursor.execute(sql, (u.getUsername(), u.getPassword(), u.getRol(), u.getNombre(), u.getEdad(), u.getSexo()))
                conn.commit()
                if cursor.rowcount > 0:
                    u.setId(cursor.lastrowid)
                    cursor.close()
                    return True
                cursor.close()
        except Exception as e:
            print(e)
        return False

    def actualizar(self, u):
        sql = "UPDATE usuarios SET username=%s, password=%s, rol=%s, nombre=%s, edad=%s, sexo=%s WHERE id=%s"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                cursor.execute(sql, (u.getUsername(), u.getPassword(), u.getRol(), u.getNombre(), u.getEdad(), u.getSexo(), u.getId()))
                conn.commit()
                res = cursor.rowcount > 0
                cursor.close()
                return res
        except Exception as e:
            print(e)
        return False

    def eliminar(self, id_usuario):
        sql = "DELETE FROM usuarios WHERE id = %s"
        try:
            conn = Conexion.getConexion()
            if conn:
                cursor = conn.cursor()
                cursor.execute(sql, (id_usuario,))
                conn.commit()
                res = cursor.rowcount > 0
                cursor.close()
                return res
        except Exception as e:
            print(e)
        return False

    def mapear(self, row):
        u = Usuario()
        u.setId(row["id"])
        u.setUsername(row["username"])
        u.setPassword(row["password"])
        u.setRol(row["rol"])
        u.setNombre(row["nombre"])
        u.setEdad(row["edad"])
        u.setSexo(row["sexo"])
        return u
