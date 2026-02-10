import { LayoutDashboard, LogOut, UserCircle } from "lucide-react";
import { useAuth } from "@/hooks/useAuth";
import { Button } from "@/components/ui";

export function Navbar() {
  const { isAuthenticated, user, logout, openLoginModal, openRegisterModal } =
    useAuth();

  return (
    <nav className="sticky top-0 z-40 border-b border-slate-200 bg-white/90 backdrop-blur-md">
      <div className="mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
        <div className="flex items-center gap-3">
          <div className="flex h-9 w-9 items-center justify-center bg-black">
            <LayoutDashboard className="h-5 w-5 text-accent-500" />
          </div>
          <span className="text-xl font-extrabold text-slate-900 tracking-tight">
            Icon
          </span>
        </div>

        <div className="flex items-center gap-3">
          {isAuthenticated && user ? (
            <>
              <div className="hidden items-center gap-2 sm:flex">
                <UserCircle className="h-5 w-5 text-slate-400" />
                <span className="text-sm font-bold text-slate-700">
                  {user.fullName}
                </span>
              </div>
              <Button variant="ghost" size="sm" onClick={logout}>
                <LogOut className="h-4 w-4" />
                <span className="hidden sm:inline">Logout</span>
              </Button>
            </>
          ) : (
            <>
              <Button variant="ghost" size="sm" onClick={openLoginModal}>
                Sign In
              </Button>
              <Button variant="primary" size="sm" onClick={openRegisterModal}>
                Sign Up
              </Button>
            </>
          )}
        </div>
      </div>
    </nav>
  );
}
