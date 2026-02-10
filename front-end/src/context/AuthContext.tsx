import {
  createContext,
  useCallback,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import { authService } from "@/api";
import type { AuthUser, LoginRequest, RegisterRequest } from "@/types";

interface AuthState {
  user: AuthUser | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}

interface AuthContextValue extends AuthState {
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
  const [state, setState] = useState<AuthState>({
    user: null,
    isAuthenticated: false,
    isLoading: true,
  });
  const [showLoginModal, setShowLoginModal] = useState(false);
  const [showRegisterModal, setShowRegisterModal] = useState(false);

  const refreshUser = useCallback(async () => {
    try {
      const user = await authService.getCurrentUser();
      setState({ user, isAuthenticated: true, isLoading: false });
    } catch {
      setState({ user: null, isAuthenticated: false, isLoading: false });
    }
  }, []);

  useEffect(() => {
    refreshUser();
  }, [refreshUser]);

  useEffect(() => {
    const handleUnauthorized = () => {
      setState({ user: null, isAuthenticated: false, isLoading: false });
      setShowLoginModal(true);
    };
    window.addEventListener("auth:unauthorized", handleUnauthorized);
    return () =>
      window.removeEventListener("auth:unauthorized", handleUnauthorized);
  }, []);

  const login = useCallback(
    async (data: LoginRequest) => {
      const user = await authService.login(data);
      setState({ user, isAuthenticated: true, isLoading: false });
      setShowLoginModal(false);
      setShowRegisterModal(false);
    },
    []
  );

  const register = useCallback(
    async (data: RegisterRequest) => {
      const user = await authService.register(data);
      setState({ user, isAuthenticated: true, isLoading: false });
      setShowLoginModal(false);
      setShowRegisterModal(false);
    },
    []
  );

  const logout = useCallback(async () => {
    await authService.logout();
    setState({ user: null, isAuthenticated: false, isLoading: false });
  }, []);

  const openLoginModal = useCallback(() => {
    setShowRegisterModal(false);
    setShowLoginModal(true);
  }, []);

  const openRegisterModal = useCallback(() => {
    setShowLoginModal(false);
    setShowRegisterModal(true);
  }, []);

  const closeAuthModals = useCallback(() => {
    setShowLoginModal(false);
    setShowRegisterModal(false);
  }, []);

  const value = useMemo<AuthContextValue>(
    () => ({
      ...state,
      login,
      register,
      logout,
      refreshUser,
      showLoginModal,
      showRegisterModal,
      openLoginModal,
      openRegisterModal,
      closeAuthModals,
    }),
    [
      state,
      login,
      register,
      logout,
      refreshUser,
      showLoginModal,
      showRegisterModal,
      openLoginModal,
      openRegisterModal,
      closeAuthModals,
    ]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
