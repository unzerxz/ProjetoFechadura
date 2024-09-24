import React from 'react';
import { Navigate } from 'react-router-dom';

const RequireAuth = ({ children, adminOnly = false }) => {
    const token = localStorage.getItem('token');
    const isAdmin = localStorage.getItem('isAdmin') === 'true';

    if (!token) {
        return <Navigate to="/login" />;
    }

    if (adminOnly && !isAdmin) {
        return <Navigate to="/login" />;
    }

    return children;
};

export default RequireAuth;