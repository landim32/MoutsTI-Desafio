import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { useForm, useFieldArray } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import {
  ArrowLeft,
  Save,
  Loader2,
  Plus,
  Trash2,
  Phone,
} from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Switch } from '@/components/ui/switch';
import { Skeleton } from '@/components/ui/skeleton';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Employee, EmployeeRole, CreateEmployeeDTO } from '@/types/employee';
import { employeeService } from '@/services/employeeService';
import { roleService } from '@/services/roleService';
import { toast } from 'sonner';

// Brazilian CPF validation
const isValidCPF = (cpf: string): boolean => {
  if (!cpf) return true; // Optional field
  const cleaned = cpf.replace(/\D/g, '');
  if (cleaned.length !== 11) return false;
  if (/^(\d)\1+$/.test(cleaned)) return false;
  
  let sum = 0;
  for (let i = 0; i < 9; i++) {
    sum += parseInt(cleaned.charAt(i)) * (10 - i);
  }
  let remainder = (sum * 10) % 11;
  if (remainder === 10 || remainder === 11) remainder = 0;
  if (remainder !== parseInt(cleaned.charAt(9))) return false;
  
  sum = 0;
  for (let i = 0; i < 10; i++) {
    sum += parseInt(cleaned.charAt(i)) * (11 - i);
  }
  remainder = (sum * 10) % 11;
  if (remainder === 10 || remainder === 11) remainder = 0;
  if (remainder !== parseInt(cleaned.charAt(10))) return false;
  
  return true;
};

const phoneSchema = z.object({
  number: z.string().min(8, 'Número inválido'),
});

const employeeSchema = z.object({
  firstName: z.string().min(1, 'Nome é obrigatório').max(50, 'Nome muito longo'),
  lastName: z.string().min(1, 'Sobrenome é obrigatório').max(50, 'Sobrenome muito longo'),
  email: z.string().min(1, 'Email é obrigatório').email('Email inválido'),
  docNumber: z.string().optional().refine((val) => !val || isValidCPF(val), 'CPF inválido'),
  birthDate: z.string().optional(),
  roleId: z.number().min(1, 'Cargo é obrigatório'),
  managerId: z.number().optional(),
  phones: z.array(phoneSchema),
  password: z.string().optional().refine((val) => !val || val.length >= 6, 'Senha deve ter no mínimo 6 caracteres'),
  confirmPassword: z.string().optional()
}).refine((data) => {
  if (!data.password && !data.confirmPassword) return true;
  return data.password === data.confirmPassword;
}, {
  message: 'As senhas não coincidem',
  path: ['confirmPassword'],
});

type EmployeeFormData = z.infer<typeof employeeSchema>;

export default function EmployeeForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEditing = !!id;

  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [roles, setRoles] = useState<EmployeeRole[]>([]);
  const [managers, setManagers] = useState<{ id: number; name: string }[]>([]);

  const {
    register,
    handleSubmit,
    control,
    watch,
    setValue,
    formState: { errors, isDirty },
    reset,
  } = useForm<EmployeeFormData>({
    resolver: zodResolver(employeeSchema),
    defaultValues: {
      firstName: '',
      lastName: '',
      email: '',
      docNumber: '',
      birthDate: '',
      roleId: 0,
      managerId: 0,
      phones: [],
      password: '',
      confirmPassword: ''
    },
  });

  const { fields, append, remove } = useFieldArray({
    control,
    name: 'phones',
  });

  useEffect(() => {
    loadInitialData();
  }, [id]);

  const loadInitialData = async () => {
    setIsLoading(true);
    try {
      const [rolesData, employeesData] = await Promise.all([
        roleService.getAll(),
        employeeService.getAll(),
      ]);


      //rolesData.map(role => {console.log('Loaded role:', role); });
      setRoles(rolesData);
      setManagers(
        employeesData
          .filter((e) => e.employeeId.toString() !== id)
          .map((e) => ({ id: e.employeeId, name: e.fullName }))
      );

      if (id) {
        const employee = await employeeService.getById(Number(id));
        reset({
          firstName: employee.firstName || '',
          lastName: employee.lastName || '',
          email: employee.email,
          docNumber: employee.docNumber || '',
          birthDate: employee.birthday?.split('T')[0] || '',
          roleId: employee.roleId,
          managerId: employee.managerId || 0,
          phones: (employee.phones || []).map(phone => ({ number: phone })),
          password: '',
          confirmPassword: ''
        });
      }
    } catch (error) {
      toast.error('Erro ao carregar dados');
      navigate('/dashboard');
    } finally {
      setIsLoading(false);
    }
  };

  const onSubmit = async (data: EmployeeFormData) => {
    setIsSaving(true);
    try {
      const payload: CreateEmployeeDTO = {
        firstName: data.firstName,
        lastName: data.lastName,
        email: data.email,
        docNumber: data.docNumber || undefined,
        birthday: data.birthDate || undefined,
        roleId: data.roleId,
        managerId: data.managerId || undefined,
        phones: data.phones.map(phone => phone.number),
        ...(data.password && { password: data.password }),
      };

      if (isEditing) {
        await employeeService.update(Number(id)!, { ...payload });
        toast.success('Funcionário atualizado com sucesso');
      } else {
        await employeeService.create(payload);
        toast.success('Funcionário criado com sucesso');
      }
      navigate('/dashboard');
    } catch (error: any) {
      const message = error.response?.data?.message || 'Erro ao salvar funcionário';
      toast.error(message);
    } finally {
      setIsSaving(false);
    }
  };

  const formatCPF = (value: string) => {
    const cleaned = value.replace(/\D/g, '').slice(0, 11);
    return cleaned
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d{1,2})$/, '$1-$2');
  };

  const handleCPFChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const formatted = formatCPF(e.target.value);
    setValue('docNumber', formatted);
  };

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Skeleton className="h-10 w-10 rounded-lg" />
          <Skeleton className="h-8 w-48" />
        </div>
        <div className="space-y-4">
          {Array.from({ length: 6 }).map((_, i) => (
            <Skeleton key={i} className="h-14 rounded-lg" />
          ))}
        </div>
      </div>
    );
  }

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      className="space-y-6 max-w-3xl"
    >
      {/* Header */}
      <div className="flex items-center gap-4">
        <Button variant="outline" size="icon" onClick={() => navigate(-1)}>
          <ArrowLeft className="h-5 w-5" />
        </Button>
        <div>
          <h1 className="text-3xl font-bold text-foreground">
            {isEditing ? 'Editar Funcionário' : 'Novo Funcionário'}
          </h1>
          <p className="text-muted-foreground">
            {isEditing ? 'Atualize as informações do funcionário' : 'Preencha os dados para cadastrar um novo funcionário'}
          </p>
        </div>
      </div>

      {/* Form */}
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        <div className="bg-card rounded-xl border border-border p-6 shadow-sm space-y-6">
          <h2 className="text-lg font-semibold">Informações Pessoais</h2>

          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="firstName">Nome *</Label>
              <Input
                id="firstName"
                {...register('firstName')}
                className={errors.firstName ? 'border-destructive' : ''}
              />
              {errors.firstName && (
                <p className="text-sm text-destructive">{errors.firstName.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="lastName">Sobrenome *</Label>
              <Input
                id="lastName"
                {...register('lastName')}
                className={errors.lastName ? 'border-destructive' : ''}
              />
              {errors.lastName && (
                <p className="text-sm text-destructive">{errors.lastName.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="email">Email *</Label>
              <Input
                id="email"
                type="email"
                {...register('email')}
                className={errors.email ? 'border-destructive' : ''}
              />
              {errors.email && (
                <p className="text-sm text-destructive">{errors.email.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="cpf">CPF</Label>
              <Input
                id="cpf"
                {...register('docNumber')}
                onChange={handleCPFChange}
                placeholder="000.000.000-00"
                className={errors.docNumber ? 'border-destructive' : ''}
              />
              {errors.docNumber && (
                <p className="text-sm text-destructive">{errors.docNumber.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="birthDate">Data de Nascimento</Label>
              <Input
                id="birthDate"
                type="date"
                {...register('birthDate')}
                className={errors.birthDate ? 'border-destructive' : ''}
              />
              {errors.birthDate && (
                <p className="text-sm text-destructive">{errors.birthDate.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="password">Senha</Label>
              <Input
                id="password"
                type="password"
                {...register('password')}
                placeholder="Mínimo 6 caracteres (opcional)"
                className={errors.password ? 'border-destructive' : ''}
              />
              {errors.password && (
                <p className="text-sm text-destructive">{errors.password.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label htmlFor="confirmPassword">Confirmar Senha</Label>
              <Input
                id="confirmPassword"
                type="password"
                {...register('confirmPassword')}
                placeholder="Repita a senha (opcional)"
                className={errors.confirmPassword ? 'border-destructive' : ''}
              />
              {errors.confirmPassword && (
                <p className="text-sm text-destructive">{errors.confirmPassword.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label>Cargo *</Label>
              <Select
                value={watch('roleId').toString()}
                onValueChange={(value) => setValue('roleId', Number(value)) }
              >
                <SelectTrigger className={errors.roleId ? 'border-destructive' : ''}>
                  <SelectValue placeholder="Selecione um cargo" />
                </SelectTrigger>
                <SelectContent>
                  {roles.map((role) => (
                    <SelectItem key={role.roleId} value={role.roleId.toString()}>
                      {role.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {errors.roleId && (
                <p className="text-sm text-destructive">{errors.roleId.message}</p>
              )}
            </div>

            <div className="space-y-2">
              <Label>Gerente</Label>
              <Select
                value={watch('managerId')?.toString() || ''}
                onValueChange={(value) => setValue('managerId', value === 'none' ? undefined : Number(value))}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Selecione um gerente (opcional)" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="none">Nenhum</SelectItem>
                  {managers.map((manager) => (
                    <SelectItem key={manager.id} value={manager.id.toString()}>
                      {manager.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>
        </div>

        {/* Phones */}
        <div className="bg-card rounded-xl border border-border p-6 shadow-sm space-y-4">
          <div className="flex items-center justify-between">
            <h2 className="text-lg font-semibold">Telefones</h2>
            <Button
              type="button"
              variant="outline"
              size="sm"
              onClick={() => append({ number: '' })}
            >
              <Plus className="h-4 w-4" />
              Adicionar
            </Button>
          </div>

          {fields.length === 0 ? (
            <p className="text-muted-foreground text-center py-6">
              Nenhum telefone adicionado
            </p>
          ) : (
            <div className="space-y-3">
              {fields.map((field, index) => (
                <motion.div
                  key={field.id}
                  initial={{ opacity: 0, x: -20 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: 20 }}
                  className="flex gap-3 items-start p-4 rounded-lg bg-muted/50"
                >
                  <div className="p-2 rounded-lg bg-primary/10 text-primary">
                    <Phone className="h-5 w-5" />
                  </div>
                  <div className="flex-1 space-y-1">
                    <Label>Número</Label>
                    <Input
                      {...register(`phones.${index}.number`)}
                      placeholder="(00) 00000-0000"
                      className={errors.phones?.[index]?.number ? 'border-destructive' : ''}
                    />
                    {errors.phones?.[index]?.number && (
                      <p className="text-xs text-destructive">
                        {errors.phones[index]?.number?.message}
                      </p>
                    )}
                  </div>
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon"
                    className="text-destructive hover:text-destructive"
                    onClick={() => remove(index)}
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </motion.div>
              ))}
            </div>
          )}
        </div>

        {/* Actions */}
        <div className="flex justify-end gap-3">
          <Button type="button" variant="outline" onClick={() => navigate(-1)}>
            Cancelar
          </Button>
          <Button type="submit" disabled={isSaving}>
            {isSaving ? (
              <Loader2 className="h-5 w-5 animate-spin" />
            ) : (
              <>
                <Save className="h-5 w-5" />
                {isEditing ? 'Salvar Alterações' : 'Criar Funcionário'}
              </>
            )}
          </Button>
        </div>
      </form>
    </motion.div>
  );
}
