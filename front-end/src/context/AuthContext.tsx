import { createContext, useEffect, useState, type ReactNode } from "react";
import { authService } from "@/api";
import type { AuthUser, LoginRequest, RegisterRequest } from "@/types";

interface AuthContextValue {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (data: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;
  refreshUser: () => Promise<void>;
  showLoginModal: boolean;
  showRegisterModal: boolean;
  openLoginModal: () => void;
  openRegisterModal: () => void;
  closeAuthModals: () => void;
}

export const AuthContext = createContext<AuthContextValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [showLoginModal, setShowLoginModal] = useState(false);
  const [showRegisterModal, setShowRegisterModal] = useState(false);

  async function refreshUser() {
    try {
      const currentUser = await authService.getCurrentUser();
      setUser(currentUser);
      setIsAuthenticated(true);
    } catch {
      setUser(null);
      setIsAuthenticated(false);
    } finally {
      setIsLoading(false);
    }
  }

  useEffect(() => {
    refreshUser();
  }, []);

  useEffect(() => {
    function handleUnauthorized() {
      setUser(null);
      setIsAuthenticated(false);
      setIsLoading(false);
      setShowLoginModal(true);
    }

    window.addEventListener("auth:unauthorized", handleUnauthorized);
    return () => window.removeEventListener("auth:unauthorized", handleUnauthorized);
  }, []);

  async function login(data: LoginRequest) {
    const loggedInUser = await authService.login(data);
    setUser(loggedInUser);
    setIsAuthenticated(true);
    setIsLoading(false);
    setShowLoginModal(false);
    setShowRegisterModal(false);
  }

  async function register(data: RegisterRequest) {
    const newUser = await authService.register(data);
    setUser(newUser);
    setIsAuthenticated(true);
    setIsLoading(false);
    setShowLoginModal(false);
    setShowRegisterModal(false);
  }

  async function logout() {
    await authService.logout();
    setUser(null);
    setIsAuthenticated(false);
    setIsLoading(false);
  }

  function openLoginModal() {
    setShowRegisterModal(false);
    setShowLoginModal(true);
  }

  function openRegisterModal() {
    setShowLoginModal(false);
    setShowRegisterModal(true);
  }

  function closeAuthModals() {
    setShowLoginModal(false);
    setShowRegisterModal(false);
  }

  const value: AuthContextValue = {
    user,
    isAuthenticated,
    isLoading,
    login,
    register,
    logout,
    refreshUser,
    showLoginModal,
    showRegisterModal,
    openLoginModal,
    openRegisterModal,
    closeAuthModals,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
