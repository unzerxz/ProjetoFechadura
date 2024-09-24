import React from 'react';
import { Link } from 'react-router-dom';

function UserDashboard() {
    return (
        <div className="user-dashboard">
            <h1>User Dashboard</h1>
            <ul className="user-menu">
                <li><Link to="/salas">Ver Salas</Link></li>
                <li><Link to="/profile">Editar Perfil</Link></li>
            </ul>
        </div>
    );
}

export default UserDashboard;