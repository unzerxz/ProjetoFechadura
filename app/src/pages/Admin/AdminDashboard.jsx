import React from 'react';
import { Link } from 'react-router-dom';

function AdminDashboard() {
    return (
        <div className="admin-dashboard">
            <h1>Admin Dashboard</h1>
            <ul className="admin-menu">
                <li><Link to="/admin/cargos">Gerenciar Cargos</Link></li>
                <li><Link to="/admin/salas">Criar Sala</Link></li>
                <li><Link to="/admin/funcionarios">Gerenciar Funcionários</Link></li>
                <li><Link to="/admin/historico">Ver Histórico</Link></li>
            </ul>
        </div>
    );
}

export default AdminDashboard;