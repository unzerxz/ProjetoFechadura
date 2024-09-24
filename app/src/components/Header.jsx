import React from 'react';
import { Link, useNavigate } from 'react-router-dom';

function Header() {
    const navigate = useNavigate();
    const isAdmin = localStorage.getItem('isAdmin') === 'true';

    const handleLogout = () => {
        localStorage.removeItem('token');
        localStorage.removeItem('isAdmin');
        navigate('/login');
    };

    return (
        <header className="header">
            <nav>
                <ul className="nav-list">
                    <li><Link to="/salas">Salas</Link></li>
                    <li><Link to="/profile">Perfil</Link></li>
                    {isAdmin && (
                        <>
                            <li><Link to="/admin/cargos">Cargos</Link></li>
                            <li><Link to="/admin/salas">Criar Sala</Link></li>
                            <li><Link to="/admin/funcionarios">Funcionários</Link></li>
                            <li><Link to="/admin/historico">Histórico</Link></li>
                        </>
                    )}
                    <li><button onClick={handleLogout} className="logout-button">Logout</button></li>
                </ul>
            </nav>
        </header>
    );
}

export default Header;