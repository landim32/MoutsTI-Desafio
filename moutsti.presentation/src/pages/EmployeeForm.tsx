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
  cpf: z.string().optional().refine((val) => !val || isValidCPF(val), 'CPF inválido'),
  birthDate: z.string().optional(),
  roleId: z.string().min(1, 'Cargo é obrigatório'),
  managerId: z.string().optional(),
  phones: z.array(phoneSchema),
  isActive: z.boolean(),
});

type EmployeeFormData = z.infer<typeof employeeSchema>;

export default function EmployeeForm() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEditing = !!id;

  const [isLoading, setIsLoading] = useState(false);
  const [isSaving, setIsSaving] = useState(false);
  const [roles, setRoles] = useState<EmployeeRole[]>([]);
  const [managers, setManagers] = useState<{ id: string; name: string }[]>([]);

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
      cpf: '',
      birthDate: '',
      roleId: '',
      managerId: '',
      phones: []
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

      setRoles(rolesData);
      setManagers(
        employeesData
          .filter((e) => e.employeeId !== id)
          .map((e) => ({ id: e.employeeId, name: e.fullName }))
      );

      if (id) {
        const employee = await employeeService.getById(id);
        reset({
          firstName: employee.firstName || '',
          lastName: employee.lastName || '',
          email: employee.email,
          cpf: employee.cpf || '',
          birthDate: employee.birthDate?.split('T')[0] || '',
          roleId: employee.roleId,
          managerId: employee.managerId || '',
          phones: (employee.phones || []).map(phone => ({ number: phone }))
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
        cpf: data.cpf || undefined,
        birthDate: data.birthDate || undefined,
        roleId: data.roleId,
        managerId: data.managerId || undefined,
        phones: data.phones.map(phone => phone.number),
      };

      if (isEditing) {
        await employeeService.update(id!, { ...payload });
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
    setValue('cpf', formatted);
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
                {...register('cpf')}
                onChange={handleCPFChange}
                placeholder="000.000.000-00"
                className={errors.cpf ? 'border-destructive' : ''}
              />
              {errors.cpf && (
                <p className="text-sm text-destructive">{errors.cpf.message}</p>
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
              <Label>Cargo *</Label>
              <Select
                value={watch('roleId')}
                onValueChange={(value) => setValue('roleId', value)}
              >
                <SelectTrigger className={errors.roleId ? 'border-destructive' : ''}>
                  <SelectValue placeholder="Selecione um cargo" />
                </SelectTrigger>
                <SelectContent>
                  {roles.map((role) => (
                    <SelectItem key={role.roleId} value={role.roleId}>
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
                value={watch('managerId') || ''}
                onValueChange={(value) => setValue('managerId', value === 'none' ? '' : value)}
              >
                <SelectTrigger>
                  <SelectValue placeholder="Selecione um gerente (opcional)" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="none">Nenhum</SelectItem>
                  {managers.map((manager) => (
                    <SelectItem key={manager.id} value={manager.id}>
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
