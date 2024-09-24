import React from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import LoginForm from './components/Auth/LoginForm';
import RegisterForm from './components/Auth/RegisterForm';
import Header from './components/Header';
import CargoList from './components/Admin/CargoList';
import CreateRoom from './components/Admin/CreateRoom';
import ManageEmployees from './components/Admin/ManageEmployees';
import ViewHistory from './components/Admin/ViewHistory';
import SalaView from './components/User/SalaView';
import UserProfile from './components/User/UserProfile';
import AdminDashboard from './pages/Admin/AdminDashboard';
import UserDashboard from './pages/User/UserDashboard';
import RequireAuth from './components/RequireAuth';
import './App.css'; // Certifique-se de que o CSS está sendo importado

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/login" element={<LoginForm />} />
                <Route path="/register" element={<RegisterForm />} />
                <Route
                    path="/salas"
                    element={
                        <RequireAuth>
                            <SalaView />
                        </RequireAuth>
                    }
                />
                <Route
                    path="/profile"
                    element={
                        <RequireAuth>
                            <UserProfile />
                        </RequireAuth>
                    }
                />
                <Route
                    path="/user-dashboard"
                    element={
                        <RequireAuth>
                            <UserDashboard />
                        </RequireAuth>
                    }
                />
                <Route
                    path="/admin-dashboard"
                    element={
                        <RequireAuth adminOnly={true}>
                            <AdminDashboard />
                        </RequireAuth>
                    }
                />
                <Route
                    path="/admin/cargos"
                    element={
                        <RequireAuth adminOnly={true}>
                            <CargoList />
                        </RequireAuth>
                    }
                />
                <Route
                    path="/admin/salas"
                    element={
                        <RequireAuth adminOnly={true}>
                            <CreateRoom />
                        </RequireAuth>
                    }
                />
                <Route
                    path="/admin/funcionarios"
                    element={
                        <RequireAuth adminOnly={true}>
                            <ManageEmployees />
                        </RequireAuth>
                    }
                />
                <Route
                    path="/admin/historico"
                    element={
                        <RequireAuth adminOnly={true}>
                            <ViewHistory />
                        </RequireAuth>
                    }
                />
                <Route path="/" element={<Navigate to="/login" />} />
            </Routes>
        </Router>
    );
}

export default App;